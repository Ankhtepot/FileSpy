using FileSpy.Services;
using JetBrains.Annotations;
using System;
using FileSpy.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FileSpy.View;
using Prism.Commands;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;

namespace FileSpy.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        public const string DEFAULT_PROGRESS_STATUS = "No task in progress.";

        private LoggingOptions loggingOptions;
        public LoggingOptions LoggingOptions
        {
            get { return loggingOptions; }
            set
            {
                loggingOptions = value;
                ScanRootDirectoryCommand.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        private string rootPath;
        public string RootPath
        {
            get { return rootPath; }
            set
            {
                rootPath = value;
                ScanRootDirectoryCommand.RaiseCanExecuteChanged();
                OutputFileName = Directory.Exists(value) ? new DirectoryInfo(value).Name : "";
                OnPropertyChanged();
            }
        }

        private string outputPath;
        public string OutputPath
        {
            get { return outputPath; }
            set
            {
                outputPath = value;
                ScanRootDirectoryCommand.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        private string fileSearchPatterns;
        public string FileSearchPatterns
        {
            get { return fileSearchPatterns; }
            set { fileSearchPatterns = value; OnPropertyChanged(); }
        }

        private string directorySearchPatterns;
        public string DirectorySearchPatterns
        {
            get { return directorySearchPatterns; }
            set { directorySearchPatterns = value; }
        }


        private string outputFileName;
        public string OutputFileName
        {
            get { return outputFileName; }
            set
            {
                outputFileName = value;
                ScanRootDirectoryCommand.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        private char delimiter;

        public char Delimiter
        {
            get { return delimiter; }
            set { delimiter = value; }
        }


        private string progressStatus;
        public string ProgressStatus
        {
            get { return progressStatus; }
            set { progressStatus = value; OnPropertyChanged(); }
        }


        private bool isWorking;
        public bool IsWorking
        {
            get { return isWorking; }
            set
            {
                isWorking = value;
                ScanRootDirectoryCommand.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        public DelegateCommand GetRootPathCommand { get; set; }
        public DelegateCommand GetDestinationPathCommand { get; set; }
        public DelegateCommand SetLoggingPropertiesCommand { get; set; }
        public DelegateCommand ScanRootDirectoryCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainVM()
        {
            GetRootPathCommand = new DelegateCommand(GetRootPath);
            GetDestinationPathCommand = new DelegateCommand(GetDestinationPath);
            SetLoggingPropertiesCommand = new DelegateCommand(SetLoggingProperties);
            ScanRootDirectoryCommand = new DelegateCommand(ScanRootDirectory, ScanRootDirectory_CanExecute);

            LoggingOptions = new LoggingOptions();
            LoggingOptions.PropertyChanged += (sender, e) => ScanRootDirectoryCommand.RaiseCanExecuteChanged();
            ProgressStatus = DEFAULT_PROGRESS_STATUS;
            Delimiter = ',';
        }

        public void GetRootPath()
        {
            RootPath = FileServices.QueryUserForPath(RootPath);
        }

        public void GetDestinationPath()
        {
            OutputPath = FileServices.QueryUserForPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        }

        public async void ScanRootDirectory()
        {
            ProgressStatus = "Reading Directories...";
            IsWorking = true;
            var directories = await Task.Run(() => GetDirectoriesTask());
            var foundDirectoryCount = directories.Count;

            var progress = new Progress<int>(itemNr =>
            {
                ProgressStatus = $"Processing Directory: {itemNr} / {foundDirectoryCount}";
            });

            var files = await Task.Run(() => GetFilesTask(directories, progress));
            var foundFilesCount = files.Count;

            if(foundFilesCount < 1)
            {
                ProgressStatus = "No files matching the criteria found.";
                IsWorking = false;
                return;
            }

            progress = new Progress<int>(fileNr => {
                ProgressStatus = $"Reading File Informations: {fileNr} / {foundFilesCount}";
            });

            var infos = await Task.Run(() => GetFileVersionInfosTask(files, progress));

            progress = new Progress<int>(fileNr =>
            {
                ProgressStatus = $"Generating Csv from file: {fileNr} / {foundFilesCount}";
            });

            var csvFileString = await Task.Run(() => GetCsvFileStringTask(infos, progress));

            FileServices.SaveCsvString(OutputFileName, OutputPath, csvFileString, LoggingOptions.AppendInsteadOfRewrite);

            IsWorking = false;
            ProgressStatus = $"Processed {foundFilesCount} files.";
        }

        private bool ScanRootDirectory_CanExecute()
        {
            return Directory.Exists(RootPath)
                && Directory.Exists(OutputPath)
                && LoggingOptions.IsAnyLoggingSelected()
                && !string.IsNullOrEmpty(OutputFileName)
                && !IsWorking;
        }

        public void SetLoggingProperties()
        {
            Console.WriteLine("SetLoggingProperties executed");
            var selectPropertiesWindow = new SelectLoggedPropertiesWindow(this);
            selectPropertiesWindow.ShowDialog();
        }

        private List<string> GetDirectoriesTask()
        {
            var result = Task.Run(async () =>
            {
                var asyncResult = await Task.Run(() => FileServices.GetDirectoriesFromRootPath(RootPath, DirectorySearchPatterns));
                return asyncResult;
            });

            return new List<string>() { RootPath }.Concat(result.Result).ToList();
        }

        private List<string> GetFilesTask(List<string> directories, IProgress<int> progress)
        {
            var result = Task.Run(async () =>
           {
               var foundFiles = new List<string>();
               for (int i = 0; i < directories.Count; i++)
               {
                   var files = await Task.Run(() => FileServices.ScanDirectory(directories[i], FileSearchPatterns));

                   if(files != null && files.Count > 0)
                   {
                       foundFiles.AddRange(files);
                   }

                   if (progress != null)
                   {
                       progress.Report(i + 1);
                   }
               }
               return foundFiles;
           });

            return result.Result;
        }

        private List<FileVersionInfo> GetFileVersionInfosTask(List<string> files, IProgress<int> progress)
        {
            var result = Task.Run(async () =>
           {
               var receivedInfos = new List<FileVersionInfo>();
               for (int i = 0; i < files.Count; i++)
               {
                   var result = await Task.Run(() => FileServices.GetFileInfos(files[i])); 

                   if (result != null)
                   {
                       receivedInfos.Add(result);
                   }

                   if(progress != null)
                   {
                       progress.Report(i + 1);
                   }
               }
               return receivedInfos;
           });

            return result.Result;
        }

        private string GetCsvFileStringTask(List<FileVersionInfo> infos, IProgress<int> progress)
        {
            var result = Task.Run(async () =>
            {
                var csvFileStringBuilder = new StringBuilder($"{CreateHeaderLineForCsv(LoggingOptions, Delimiter)}\n");

                for (int i = 0; i < infos.Count; i++)
                {
                    var result = await Task.Run(() => StringServices.TransformInfoToCsvLineString(infos[i], RootPath, LoggingOptions, Delimiter));

                    if (result != null)
                    {
                        csvFileStringBuilder.Append(result + '\n');
                    }

                    if (progress != null)
                    {
                        progress.Report(i + 1);
                    }
                }
                return csvFileStringBuilder;
            });

            return result.Result.ToString();
        }

        private string CreateHeaderLineForCsv(LoggingOptions loggedProperties, char delimiter)
        {
            var result = new StringBuilder("Relative Path" + delimiter);
            if (loggedProperties.LogFileVersion) result.Append("File Version" + delimiter);
            if (loggedProperties.LogProductVersion) result.Append("Product Version" + delimiter);

            return StringServices.RemoveRedundantDelimiter(result.ToString(), delimiter);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

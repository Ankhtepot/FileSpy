using FileSpy.Services;
using FileSpy.ViewModel.Commands;
using JetBrains.Annotations;
using System;
using FileSpy.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using static FileSpy.Model.Enums;
using FileSpy.Model.Interfaces;
using FileSpy.View;
using Microsoft.WindowsAPICodePack.Shell;
using Prism.Commands;
using System.IO;
using CSVEditor.ViewModel.BackgroundWorkers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace FileSpy.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        public const string DEFAULT_PROGRESS_STATUS = "No task in progress.";

        private LoggedProperties loggedProperties;
        public LoggedProperties LoggedProperties
        {
            get { return loggedProperties; }
            set
            {
                loggedProperties = value;
                ScanRootDirectoryCommandDel.RaiseCanExecuteChanged();
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
                ScanRootDirectoryCommandDel.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        private string destinationPath;
        public string DestinationPath
        {
            get { return destinationPath; }
            set
            {
                destinationPath = value;
                ScanRootDirectoryCommandDel.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        private string searchPattern;
        public string SearchPattern
        {
            get { return searchPattern; }
            set { searchPattern = value; OnPropertyChanged(); }
        }

        private string outputFileName;
        public string OutputFileName
        {
            get { return outputFileName; }
            set
            {
                outputFileName = value;
                ScanRootDirectoryCommandDel.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        private string progressStatus;

        public string ProgressStatus
        {
            get { return progressStatus; }
            set { progressStatus = value; OnPropertyChanged(); }
        }


        private WorkStatus workingStatus;
        public WorkStatus WorkingStatus
        {
            get { return workingStatus; }
            set
            {
                workingStatus = value;
                ScanRootDirectoryCommandDel.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        private IWorker activeWorker;
        public IWorker ActiveWorker
        {
            get { return activeWorker; }
            set { activeWorker = value; }
        }

        public GetRootPathCommand GetRootPathCommand { get; set; }
        public GetDestinationPathCommand GetDestinationPathCommand { get; set; }
        public SetLoggingPropertiesCommand SetLoggingPropertiesCommand { get; set; }

        public DelegateCommand ScanRootDirectoryCommandDel { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainVM()
        {
            GetRootPathCommand = new GetRootPathCommand(this);
            GetDestinationPathCommand = new GetDestinationPathCommand(this);
            SetLoggingPropertiesCommand = new SetLoggingPropertiesCommand(this);
            ScanRootDirectoryCommandDel = new DelegateCommand(ScanRootDirectory, CanScanRootDirectory);

            LoggedProperties = new LoggedProperties();
            LoggedProperties.SetAllPropertiesTo(true);

            ProgressStatus = DEFAULT_PROGRESS_STATUS;
        }

        public void GetRootPath()
        {
            RootPath = FileServices.QueryUserForPath();
        }

        public void GetDestinationPath()
        {
            DestinationPath = FileServices.QueryUserForPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        }

        public async void ScanRootDirectory()
        {
            Console.WriteLine("ScanRootDirectory executed");
            ProgressStatus = "Reading Directories...";
            WorkingStatus = WorkStatus.Working;
            var directories = await Task.Run(() => GetDirectoriesTask());
            var foundDirectoryCount = directories.Count;

            var progress = new Progress<int>(itemNr =>
            {
                ProgressStatus = $"Processing Directory: {itemNr} / {foundDirectoryCount}";
            });

            var files = await Task.Run(() => GetFilesTask(directories, progress));
            var foundFilesCount = files.Count;

            progress = new Progress<int>(fileNr => {
                ProgressStatus = $"Reading File Informations: {fileNr} / {foundFilesCount}";
            });

            var infos = await Task.Run(() => GetFileVersionInfosTask(files, progress));

            progress = new Progress<int>(fileNr =>
            {
                ProgressStatus = $"Generating Excel from file: {fileNr} / {foundDirectoryCount}";
            });

            WorkingStatus = WorkStatus.Idle;
        }

        private bool CanScanRootDirectory()
        {
            return Directory.Exists(RootPath)
                && Directory.Exists(DestinationPath)
                && (LoggedProperties.LogProductVersion || LoggedProperties.LogFileVersion)
                && !string.IsNullOrEmpty(OutputFileName)
                && WorkingStatus != WorkStatus.Working;
        }

        public void SetLoggingProperties()
        {
            Console.WriteLine("SetLoggingProperties executed");
            var selectPropertiesWindow = new SelectLoggedPropertiesWindow(this);
            selectPropertiesWindow.ShowDialog();

            LoggedProperties.LogFileVersion = selectPropertiesWindow.LogFileVersion;
            LoggedProperties.LogProductVersion = selectPropertiesWindow.LogProductVersion;
        }

        private List<string> GetDirectoriesTask()
        {
            var result = Task.Run(async () =>
            {
                var asyncResult = await Task.Run(() => FileServices.GetDirectoriesFromRootPath(RootPath));
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
                   var files = await Task.Run(() => FileServices.ScanDirectory(rootPath, directories[i], SearchPattern));

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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

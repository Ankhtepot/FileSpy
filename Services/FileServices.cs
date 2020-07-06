using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileSpy.Services
{
    public class FileServices
    {
        public static string QueryUserForPath(string initialDirectory = "")
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if(!string.IsNullOrEmpty(initialDirectory) && Directory.Exists(initialDirectory))
            {
                dialog.InitialDirectory = initialDirectory;
            }

            CommonFileDialogResult result = CommonFileDialogResult.None;
            try
            {
                result = dialog.ShowDialog();
            }
            catch (Exception e)
            {
                return $"Error while reading directory path: {e.Message}";
            }

            if (result == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            else
            {
                return "No path was set.";
            }
        }

        public static List<string> GetDirectoriesFromRootPath(string rootPath, string directorySearchPatterns = "", BackgroundWorker worker = null)
        {
            if (!Directory.Exists(rootPath))
            {
                return null;
            }

            var searchPatterns = directorySearchPatterns?.Split('|') ?? new string[] { "" };

            List<string> directories = getDirectoriesFromPath(rootPath, worker) ?? new List<string>();

            directories = directories.Where(dir => StringServices.IsTextIncludingPatterns(dir, searchPatterns)).ToList();

            foreach (var directory in directories)
            {
                var recursiveYield = GetDirectoriesFromRootPath(directory, directorySearchPatterns, worker);
                directories = recursiveYield != null
                    ? directories.Concat(recursiveYield).ToList()
                    : null;
            }

            return directories != null ? directories.ToList() : null;
        }       

        public static List<string> ScanDirectory(string path, string fileSearchPatterns = "")
        {
            List<string> foundFiles = null;
            var searchPatterns = fileSearchPatterns?.Split('|') ?? new string[] { "" };
            try
            {
                foundFiles = new List<string>(Directory.GetFiles(path));
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Insufficient rights to read all files in \"{path}\".");
            }    
            catch (Exception e)
            {
                Console.WriteLine($"Unknown error while reading a directory: {e.Message}");
            }

            foundFiles = foundFiles?.Where(file => StringServices.IsTextIncludingPatterns(file, searchPatterns)).ToList();

            return (foundFiles != null && foundFiles.Count > 0)
                ? foundFiles
                : null;
        }        

        public static FileVersionInfo GetFileInfos(string filePath)
        {
            return File.Exists(filePath) ? FileVersionInfo.GetVersionInfo(filePath) : null;
        }        

        public static bool SaveCsvString(string fileName, string path, string text, bool append = false)
        {
            var fullPath = Path.Combine(path, fileName + ".csv");

            try
            {
                if (append) {
                    File.AppendAllText(fullPath, '\n' + text);
                }
                else
                {
                    File.WriteAllText(fullPath, text);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving CSV file: {e.Message}");
                return false;
            }

            return true;
        }

        private static List<string> getDirectoriesFromPath(string path, BackgroundWorker worker = null)
        {
            if (worker != null && worker.CancellationPending)
            {
                return null;
            }

            List<string> directories = new List<string>();

            try
            {
                directories = Directory.GetDirectories(path).ToList();
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Insufficient rights to scan direcotry at \"{path}\".");
            }

            return directories;
        }
    }
}

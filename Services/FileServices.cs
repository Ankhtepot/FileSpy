using FileSpy.Model;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FileSpy.Services
{
    public class FileServices
    {
        public static string QueryUserForPath(string initialDirectory = "")
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if(!string.IsNullOrEmpty(initialDirectory))
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
                return "No root path set.";
            }
        }

        public static bool IsDirectoryWithGitRepository(string rootPath)
        {
            List<string> rootPathDirectories = new List<string>();

            try
            {
                rootPathDirectories = Directory.GetDirectories(rootPath, @".git").ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while checking for GitRepo: " + e.Message);
            }

            if (rootPathDirectories != null && rootPathDirectories.Count > 0)
            {
                foreach (var directory in rootPathDirectories)
                {
                    if (Regex.Match(directory, "\\.git").Success)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static List<string> GetDirectoriesFromRootPath(string rootPath, BackgroundWorker worker = null)
        {
            if (!Directory.Exists(rootPath))
            {
                return null;
            }

            IEnumerable<string> directories = getDirectoriesFromPath(rootPath, worker) ?? new List<string>();

            foreach (var directory in directories.Where(dir => !Regex.Match(dir, "\\.git").Success))
            {
                var recursiveYield = GetDirectoriesFromRootPath(directory, worker);
                directories = recursiveYield != null
                    ? directories.Concat(recursiveYield).ToList()
                    : null;
            }

            return directories != null ? directories.ToList() : null;
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

        public static List<string> ScanDirectory(string rootPath, string path, string regexPattern = "")
        {
            List<string> foundFiles = null;
            var searchPattern = regexPattern == null ? "*." : regexPattern;
            try
            {
                foundFiles = new List<string>(Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly));
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Insufficient rights to read all files in \"{path}\".");
            }    
            catch (Exception e)
            {
                Console.WriteLine($"Unknown error while reading a directory: {e.Message}");
            }            

            return (foundFiles != null && foundFiles.Count > 0)
                ? foundFiles
                : null;
        }

        internal static FileVersionInfo GetFileInfos(string filePath)
        {
            return File.Exists(filePath) ? FileVersionInfo.GetVersionInfo(filePath) : null;

            //var fileVersion = loggedProperties.LogFileVersion ? fileInfo.FileVersion : "";
            //var productVersion = loggedProperties.LogProductVersion ? fileInfo.ProductVersion : "";

            //var relativePath = Regex.Replace(filePath, Regex.Escape(rootPath), ".");
            //relativePath = relativePath == "." ? "Root Directory" : relativePath;
        }
    }
}

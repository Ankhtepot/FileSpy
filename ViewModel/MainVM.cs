using FileSpy.Services;
using FileSpy.ViewModel.Commands;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace FileSpy.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        private string rootPath;

        public string RootPath
        {
            get { return rootPath; }
            set { rootPath = value; OnPropertyChanged(); }
        }

        private string destinationPath;

        public string DestinationPath
        {
            get { return destinationPath; }
            set { destinationPath = value; OnPropertyChanged(); }
        }


        public GetRootPathCommand GetRootPathCommand { get; set; }
        public GetDestinationPathCommand GetDestinationPathCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainVM()
        {
            GetRootPathCommand = new GetRootPathCommand(this);
            GetDestinationPathCommand = new GetDestinationPathCommand(this);
        }

        public void GetRootPath()
        {
            RootPath = FileSystemServices.QueryUserForPath();
        }

        public void GetDestinationPath()
        {
            DestinationPath = FileSystemServices.QueryUserForPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

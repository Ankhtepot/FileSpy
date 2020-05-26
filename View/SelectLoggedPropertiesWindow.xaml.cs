using FileSpy.Model;
using FileSpy.ViewModel;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileSpy.View
{
    /// <summary>
    /// Interaction logic for SelectLoggedPropertiesWindow.xaml
    /// </summary>
    public partial class SelectLoggedPropertiesWindow : Window, INotifyPropertyChanged
    {
        private MainVM MainVM;

        private LoggedProperties properties;
        public LoggedProperties Properties
        {
            get { return properties; }
            set 
            {
                properties = value;
                OnPropertyChanged(); 
            }
        }

        private bool logProductVersion;

        public bool LogProductVersion
        {
            get { return logProductVersion; }
            set { logProductVersion = value; Properties.LogProductVersion = value; OnPropertyChanged(nameof(Properties)); }
        }

        private bool logFileVersion;

        public bool LogFileVersion
        {
            get { return logFileVersion; }
            set { logFileVersion = value; Properties.LogFileVersion = value; OnPropertyChanged(); }
        }


        public SelectLoggedPropertiesWindow(MainVM mainVM)
        {
            InitializeComponent();
            MainVM = mainVM ?? throw new ArgumentNullException(nameof(mainVM));
            Properties = MainVM.LoggedProperties;

            LogFileVersion = Properties.LogFileVersion;
            LogProductVersion = Properties.LogProductVersion;

            TopContainer.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

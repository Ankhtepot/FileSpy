using JetBrains.Annotations;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace FileSpy.Model
{
    [AddINotifyPropertyChangedInterface]
    public class LoggingOptions : INotifyPropertyChanged
    {
        private bool logFileVersion;
        public bool LogFileVersion
        {
            get { return logFileVersion; }
            set 
            {
                logFileVersion = value; 
                OnPropertyChanged(); 
            }
        }

        private bool logProductVersion;
        public bool LogProductVersion
        {
            get { return logProductVersion; }
            set 
            {
                logProductVersion = value;
                OnPropertyChanged(); 
            }
        }

        private bool appendInsteadOfRewrite;
        public bool AppendInsteadOfRewrite
        {
            get { return appendInsteadOfRewrite; }
            set { appendInsteadOfRewrite = value; }
        }

        public LoggingOptions()
        {
            LogFileVersion = true;
            LogProductVersion = true;
            AppendInsteadOfRewrite = false;
        }

        public bool IsAnyLoggingSelected()
        {
            return LogFileVersion
                || LogProductVersion;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

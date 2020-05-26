using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace FileSpy.Model
{
    public class LoggedProperties : INotifyPropertyChanged
    {
        //public bool LogFileVersion { get; set; }

        private bool logFileVersion;

        public bool LogFileVersion
        {
            get { return logFileVersion; }
            set { logFileVersion = value; OnPropertyChanged(); }
        }

        //public bool LogProductVersion { get; set; }

        private bool logProductVersion;

        public bool LogProductVersion
        {
            get { return logProductVersion; }
            set { logProductVersion = value; OnPropertyChanged(); }
        }


        private Dictionary<string, bool> properties;

        public Dictionary<string, bool> Properties
        {
            get { return properties; }
            set 
            {
                properties = value; 
                OnPropertyChanged(); 
            }
        }


        public LoggedProperties()
        {
            LogFileVersion = true;
            LogProductVersion = true;

            Properties = new Dictionary<string, bool>();
            Properties.Add("Log File Version", true);
            Properties.Add("Log Product Version", true);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetAllPropertiesTo(bool state)
        {
            var keys = Properties.Keys.ToList();
            foreach(var key in keys) 
            { 
                Properties[key] = state;
            }
        }
    }
}

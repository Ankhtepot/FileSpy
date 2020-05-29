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

        public SelectLoggedPropertiesWindow(MainVM mainVM)
        {
            InitializeComponent();
            MainVM = mainVM ?? throw new ArgumentNullException(nameof(mainVM));

            TopContainer.DataContext = MainVM.LoggingOptions;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

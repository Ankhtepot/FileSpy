using FileSpy.ViewModel;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for FileSpyWindow.xaml
    /// </summary>
    public partial class FileSpyWindow : Window
    {
        public MainVM MainVM;
        public FileSpyWindow()
        {
            InitializeComponent();
            MainVM = new MainVM();
            DataContext = MainVM;
        }
    }
}

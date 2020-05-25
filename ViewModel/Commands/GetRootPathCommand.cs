using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace FileSpy.ViewModel.Commands
{
    public class GetRootPathCommand : ICommand
    {
        private MainVM VM;

        public event EventHandler CanExecuteChanged;

        public GetRootPathCommand(MainVM vM)
        {
            VM = vM ?? throw new ArgumentNullException(nameof(vM));
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            VM.GetRootPath();
        }
    }
}

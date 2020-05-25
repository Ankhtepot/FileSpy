using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace FileSpy.ViewModel.Commands
{
    public class GetDestinationPathCommand : ICommand
    {
        private MainVM VM;

        public event EventHandler CanExecuteChanged;

        public GetDestinationPathCommand(MainVM vM)
        {
            VM = vM ?? throw new ArgumentNullException(nameof(vM));
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            VM.GetDestinationPath();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace FileSpy.ViewModel.Commands
{
    public class SetLoggingPropertiesCommand : ICommand
    {
        private MainVM VM;

        public event EventHandler CanExecuteChanged;

        public SetLoggingPropertiesCommand(MainVM vM)
        {
            VM = vM ?? throw new ArgumentNullException(nameof(vM));
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            VM.SetLoggingProperties();
        }
    }
}

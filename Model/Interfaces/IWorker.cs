using System;
using System.Collections.Generic;
using System.Text;

namespace FileSpy.Model.Interfaces
{
    public interface IWorker
    {
        public void RunAsync(object argument);
        public void CancelAsync();
    }
}

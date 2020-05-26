using FileSpy.Model;
using FileSpy.ViewModel;
using FileSpy.ViewModel.Abstracts;
using System.ComponentModel;
using static FileSpy.Model.Enums;

namespace CSVEditor.ViewModel.BackgroundWorkers
{
    public class GetCsvFileFromPathWorker : AbstractEditorVMWorker
    {
        public GetCsvFileFromPathWorker(MainVM vM) : base(vM) {}

        protected override void _DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            //worker.ReportProgress(VM.WorkProgress + 100);
            VM.WorkingStatus = WorkStatus.Working;
            //VM.SelectedCsvFile = new CsvFile((string)e.Argument, worker);

            if (worker.CancellationPending == true)
            {
                e.Cancel = true;
            }
        }

        protected override void _ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //VM.WorkProgress += 100;
        }
    }
}


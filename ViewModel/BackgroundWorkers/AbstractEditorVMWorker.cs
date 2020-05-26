﻿using FileSpy.Model;
using FileSpy.Model.Interfaces;
using System;
using System.ComponentModel;
using static FileSpy.Model.Enums;

namespace FileSpy.ViewModel.Abstracts
{
    public abstract class AbstractEditorVMWorker : IWorker
    {
        public BackgroundWorker Worker;

        protected MainVM VM { get; set; }

        public AbstractEditorVMWorker(MainVM vM)
        {
            VM = vM ?? throw new ArgumentNullException(nameof(vM));

            Worker = new BackgroundWorker();
            VM.ActiveWorker = this;
            Worker.DoWork += _DoWork;
            Worker.WorkerReportsProgress = true;
            Worker.WorkerSupportsCancellation = true;
            Worker.ProgressChanged += _ProgressChanged;
            Worker.RunWorkerCompleted += _Completed;
        }

        public void RunAsync(object forPath)
        {
            Worker.RunWorkerAsync((string)forPath);
        }

        public void CancelAsync()
        {
            Worker.CancelAsync();
        }

        protected abstract void _DoWork(object sender, DoWorkEventArgs e);

        protected abstract void _ProgressChanged(object sender, ProgressChangedEventArgs e);        

        private void _Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            var resultInfo = "";
            if (e.Cancelled == true)
            {
                resultInfo = "Canceled!";
                VM.WorkingStatus = WorkStatus.Canceled;
            }
            else if (e.Error != null)
            {
                resultInfo = "Error: " + e.Error.Message;
                VM.WorkingStatus = WorkStatus.Error;
            }
            else
            {
                resultInfo = "Done!";
                VM.WorkingStatus = WorkStatus.Done;
            }

            Console.WriteLine($"BW:{GetType().Name} - Completed status: {resultInfo}");
            VM.ActiveWorker = null;
        }

        public void CancelWorker()
        {
            Worker.DoWork -= _DoWork;
            Worker.DoWork += Cancel_DoWork;
        }

        private void Cancel_DoWork(object o, DoWorkEventArgs e)
        {
            e.Cancel = true;
        }
    }
}

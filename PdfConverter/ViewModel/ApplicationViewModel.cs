using PdfConverter.Worker;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PdfConverter
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private Certificate certificate;
        private string processStage;
        private double progress;
        private bool processError;
        

        public Certificate Certificate
        {
            get { return certificate; }
            set
            {
                certificate = value;
                OnPropertyChanged("Certificate");
            }
        }
        public string ProcessStage
        {
            get { return processStage; }
            set
            {
                processStage = value;
                OnPropertyChanged("ProcessStage");
            }
        }

        public double Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                OnPropertyChanged("Progress");
            }
        }

        public bool ProcessError
        {
            get { return processError; }
            set
            {
                processError = value;
                OnPropertyChanged("ProcessError");
            }
        }

        

        private RelayCommand convertCommand;
        public RelayCommand ConvertCommand
        {
            get
            {
                return convertCommand ??
                    (convertCommand = new RelayCommand(obj =>
                    {
                        
                        try
                        {
                            ConvertToPdfAsync();
                        }
                        catch (TransformError e)
                        {
                            ProcessError = true;
                            ProcessStage = "Ошибка преобразования:\n" + e.Message;
                            return;
                        }
                    }, _ =>
                    {
                        return Certificate != null && Certificate.FilePath != "" && !Certificate.HasValidationErrors;
                    }));
            }
        }

        private PdfDocumentTransformer transform;
        

        public ApplicationViewModel()
        {
            Certificate = new Certificate();

            transform = new PdfDocumentTransformer();

            processStage = "Заполнение карточки";
            Progress = 0;
            processError = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void ConvertToPdfAsync()
        {
            BackgroundWorker worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync(10000);


        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            transform.ProcessDocument(Certificate, sender as BackgroundWorker, e);
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
            if (e.UserState != null)
            {
                if (e.UserState is State)
                {
                    var state = (State)e.UserState;
                    if (state.IsError)
                    {
                        ProcessStage = $"{ProcessStage} {state.Message}";
                        ProcessError = state.IsError;
                    }
                    else
                    {
                        ProcessStage = state.Message;
                        ProcessError = state.IsError;
                    }
                    
                }
                
                
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!ProcessError)
            {
                ProcessStage = $"{ProcessStage} Документ успешно обработан и сохранен с именем: {e.Result}";
                ProcessError = false;

            }


        }
    }
}
using FileInputBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;

namespace PdfConverter
{
    public class Certificate : INotifyPropertyChanged, IDataErrorInfo
    {
        private string fio;
        private string fingerprint;
        private DateTime? dateFrom;
        private DateTime? dateTo;
        private string filePath;

        private Dictionary<string, string> validationErrors = new Dictionary<string, string>();

        public string Fio
        {
            get { return fio; }
            set
            {
                fio = value;
                OnPropertyChanged("Fio");
            }
        }
        public string Fingerprint
        {
            get { return fingerprint; }
            set
            {
                fingerprint = value;
                OnPropertyChanged("Fingerprint");
            }
        }
        public DateTime? DateFrom
        {
            get { return dateFrom; }
            set
            {
                dateFrom = value;
                OnPropertyChanged("DateFrom");
            }
        }
        public DateTime? DateTo
        {
            get { return dateTo; }
            set
            {
                dateTo = value;
                OnPropertyChanged("DateTo");
            }
        }
        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                OnPropertyChanged("FilePath");
            }
        }

        public string Error 
        {
            get
            {
                var propertyInfos = GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

                foreach (var propertyInfo in propertyInfos)
                {
                    var errorMsg = this[propertyInfo.Name];
                    if (null != errorMsg)
                    {
                        return errorMsg;
                    }
                }

                return null;
            }
        }

        public bool HasValidationErrors
        {
            get => validationErrors.Count > 0;
        }

        public string this[string columnName] 
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case "DateFrom":
                        if (DateFrom == null)
                        {
                            error = "Необходимо указать дату";
                        }
                        else if (DateTo != null && DateFrom > DateTo)
                        {
                            error = "Нарушение пространственно-временного континуума";
                        }
                                               
                        break;
                    case "DateTo":
                        if (DateTo == null)
                        {
                            error = "Необходимо указать дату";
                        }
                        else if (DateFrom != null && DateFrom > DateTo)
                        {
                            error = "Нарушение пространственно-временного континуума";
                        }

                        break;
                    case "Fio":
                        if (string.IsNullOrWhiteSpace(Fio)) 
                        {
                            error = "Необходимо заполнить ФИО";
                        }
                        else
                        {
                            if (!Fio.Contains(" "))
                            {
                                error = "Требуются как минимум фамилия и имя, разделенные пробелом";
                            }
                            else
                            {
                                string pattern = @"^(?=.{1,40}$)[а-яёА-ЯЁ]+(?:[-' ][а-яёА-ЯЁ]+)*$";
                                var parts = Fio.Split(new[] {' '});
                                if (parts.Length > 1)
                                {
                                    int i = 1;
                                    foreach (var part in parts)
                                    {
                                        var subparts = part.Split(new[] { '\'', '-' });
                                        bool success = true;
                                        foreach (var subpart in subparts)
                                        {
                                            var m = Regex.IsMatch(subpart, pattern);
                                            if (!m)
                                            {
                                                error = $"ФИО не соответствует шаблону: {part} содержит недопустимые символы ";
                                                success = false;
                                                break;
                                            }
                                            
                                        }
                                        i++;
                                        if (!success)
                                        {
                                            break;
                                        }
                                    }
                                }                            
                            }
                        }
                        break;
                    case "Fingerprint":
                        if (string.IsNullOrWhiteSpace(Fingerprint))
                        {
                            error = "Необходимо заполнить номер сертификата";
                        }
                        break;
                    case "FilePath":
                        if (string.IsNullOrWhiteSpace(FilePath))
                        {
                            error = "Необходимо выбрать документ";
                        }
                        break;

                }

                if (String.IsNullOrEmpty(error))
                {
                    validationErrors.Remove(columnName);
                }
                else
                {
                    validationErrors[columnName] = error;
                }
                
                

                return error;
            }
        
        
        }

        static Certificate()
        {
            
        }

        
    
        public Certificate()
        {
            fio = string.Empty;
            fingerprint = string.Empty;
            dateFrom = null;
            dateTo = null;
            filePath = string.Empty;
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

       

        


        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
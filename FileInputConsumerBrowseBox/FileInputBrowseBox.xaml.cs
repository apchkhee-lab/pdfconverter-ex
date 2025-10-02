using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Win32;

namespace FileInputBox
{
    [ContentProperty("FileName")]
    public partial class FileInputBrowseBox : UserControl
    {
        public FileInputBrowseBox()
        {
            InitializeComponent();
            theTextBox.TextChanged += new TextChangedEventHandler(OnTextChanged);
        }
        private void theButton_Click(object sender, RoutedEventArgs e)
        {
            ChooseFileName();

        }

        private string GetFileName()
        {
            OpenFileDialog d = new OpenFileDialog
            {
                Filter =
             "All Supported Files|*.docx;*.tiff;*.pdf"+
             "|PDF Documents|*.pdf" +
             "|Word Documents|*.docx" +
             "|TIFF images|*.tiff"
             
            };
            if (d.ShowDialog() == true)
            {
                return d.FileName;
            }
            return null;
        }

        private void ChooseFileName()
        {
            string fileName = GetFileName();
            if (!String.IsNullOrWhiteSpace(fileName))
            {
                FileName = fileName;

                
            }
        }

        

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        

        



       

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            FileName = theTextBox.Text;
            e.Handled = true;
            RoutedEventArgs args = new RoutedEventArgs(FileNameChangedEvent);
            RaiseEvent(args);
        }
        public event RoutedEventHandler FileNameChanged
        {
            add { AddHandler(FileNameChangedEvent, value); }
            remove { RemoveHandler(FileNameChangedEvent, value); }
        }
        public static readonly DependencyProperty FileNameProperty =
           DependencyProperty.Register("FileName", typeof(string), typeof(FileInputBrowseBox));

        


        public static readonly RoutedEvent FileNameChangedEvent =
            EventManager.RegisterRoutedEvent("FileNameChanged",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FileInputBrowseBox));


        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (oldContent != null)
                throw new InvalidOperationException("You cannot change Content");
        }
    }
}
using FileInputBox;
using System;
using System.Windows;

namespace PdfConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture= new System.Globalization.CultureInfo("ru-RU");

            InitializeComponent();

            DataContext = new ApplicationViewModel();

        }

        
    }
}

using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace TextEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Timer timer;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
            if (sfd.ShowDialog() == true)
            {
                TextRange doc = new TextRange(docBox.Document.ContentStart, docBox.Document.ContentEnd);
                using (FileStream fs = File.Create(sfd.FileName))
                {
                    //if (System.IO.Path.GetExtension(sfd.FileName).ToLower() == ".txt")
                    //{
                        doc.Save(fs, DataFormats.Text);
                    //}
                }
            }
        }

        public void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "RichText Files (*.txt)|*.txt|All files (*.*)|*.*";

            if (ofd.ShowDialog() == true)
            {
                TextRange doc = new TextRange(docBox.Document.ContentStart, docBox.Document.ContentEnd);
                using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
                {
                    //if (System.IO.Path.GetExtension(ofd.FileName).ToLower() == ".txt")
                    //{
                        doc.Load(fs, DataFormats.Text);
                    //}
                }
            }
        }

        public void Font_Click(object sender, RoutedEventArgs e)
        {
            UserControlFontDialog userControl = new UserControlFontDialog();
            var fd = userControl.fontDialog;
            var result = fd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                docBox.FontFamily = new FontFamily(fd.Font.Name);
                docBox.FontSize = fd.Font.Size * 96.0 / 72.0;
                docBox.FontWeight = fd.Font.Bold ? FontWeights.Bold : FontWeights.Regular;
                docBox.FontStyle = fd.Font.Italic ? FontStyles.Italic : FontStyles.Normal;

            }
        }
        public void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void RunThread(object state)
        {
            // использовать свободный поток из пула потоков в операционной системе
            ThreadPool.QueueUserWorkItem(PrintTime);

            // создать и использовать свой поток
            //var thread = new Thread(PrintTime);
            //// thread.SetApartmentState(System.Threading.ApartmentState.STA);
            //thread.IsBackground = true;  //фоновый поток
            //thread.Start();

        }

        public void PrintTime(object state)
        {
            // Сохранение в "Мои документы"
            string writePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\copy.txt";
            TextRange doc = new TextRange(docBox.Document.ContentStart, docBox.Document.ContentEnd);
            using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(doc.Text);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new Timer(RunThread, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(10));   // таймер на 10 секунд
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timer.Dispose();
        }
    }

}

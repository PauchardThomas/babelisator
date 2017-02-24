using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Babelisator
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ListBoxItem _dragged;
        public MainWindow()
        {
            InitializeComponent();
            loadList();

        }

        private void loadList()
        {
           if(jsxpath.Text != null)
            {
                string[] files = Directory.GetFiles(jsxpath.Text, "*.js*")
                     .Select(System.IO.Path.GetFileName)
                     .ToArray();
                foreach (var file in files)
                {
                    ListBoxItem li = new ListBoxItem();
                    li.Content = file;
                    jsxfiles.Items.Add(li);
                }
            }
        }

        private void jsFolder_Click(object sender, RoutedEventArgs e)
        {
            var path = opendialog();
            if (path != null)
            {
                jspath.Text = path;
                loadList();
            }
        }

        private void jsxFolder_Click(object sender, RoutedEventArgs e)
        {
            var path = opendialog();
            if (path != null)
            {
                jsxpath.Text = path;
            }
        }

        public string opendialog()
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            return dialog.SelectedPath;
        }

        static void write(string message)
        {
            try
            {
                using (Process p = new Process())
                {
                    // set start info
                    p.StartInfo = new ProcessStartInfo("cmd.exe")
                    {
                        RedirectStandardInput = true,
                        UseShellExecute = false,
                        WorkingDirectory = @"C:\Users\tpauchard\Desktop"
                    };
                    // event handlers for output & error
                    p.OutputDataReceived += p_OutputDataReceived;
                    p.ErrorDataReceived += p_ErrorDataReceived;

                    // start process
                    p.Start();
                    // send command to its input
                    // p.StandardInput.Write("dir");
                    p.StandardInput.Write(message + p.StandardInput.NewLine);
                    //wait
                    // p.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Process p = sender as Process;
            if (p == null)
                return;
            Console.WriteLine(e.Data);
        }

        static void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Process p = sender as Process;
            if (p == null)
                return;
            Console.WriteLine(e.Data);
        }

        private void convert_Click(object sender, RoutedEventArgs e)
        {
            if(jspath != null && jsxpath != null)
            {
                foreach (ListBoxItem item in toconvert.Items)
                {
                    string fileToConvert = item.Content.ToString();
                    Console.WriteLine(fileToConvert);
                    Console.WriteLine(babelpath + " " + jsxpath.Text + "\\" + fileToConvert + " --out-file " + jspath.Text + "\\" + fileToConvert);
                    write(babelpath + " " + jsxpath.Text + "\\" + fileToConvert + " --out-file " + jspath.Text + "\\" + fileToConvert);
                }
               
            }else
            {
                System.Windows.MessageBox.Show("L'un des chemins est vide");
            }

        }

        private void jsfiles_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_dragged != null)
                return;

            UIElement element = jsxfiles.InputHitTest(e.GetPosition(jsxfiles)) as UIElement;

            while (element != null)
            {
                if (element is ListBoxItem)
                {
                    _dragged = (ListBoxItem)element;
                    break;
                }
                element = VisualTreeHelper.GetParent(element) as UIElement;
            }
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_dragged == null)
                return;
            if (e.LeftButton == MouseButtonState.Released)
            {
                _dragged = null;
                return;
            }

            System.Windows.DataObject obj = new System.Windows.DataObject(System.Windows.DataFormats.Text, _dragged.ToString());
            DragDrop.DoDragDrop(_dragged, obj, System.Windows.DragDropEffects.All);
        }

        private void toconvert_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (_dragged == null || e.Data.GetDataPresent(System.Windows.DataFormats.Text, true) == false)
                e.Effects = System.Windows.DragDropEffects.None;
            else
                e.Effects = System.Windows.DragDropEffects.All;
        }

        private void toconvert_DragOver(object sender, System.Windows.DragEventArgs e)
        {

        }

        private void toconvert_Drop(object sender, System.Windows.DragEventArgs e)
        {
            jsxfiles.Items.Remove(_dragged);
            toconvert.Items.Add(_dragged);
        }

        private void babelFile_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.ShowDialog();
            string path = ofd.FileName;
            if (path != null)
            {
                babelpath.Text = path;
            }
        }
    }
}

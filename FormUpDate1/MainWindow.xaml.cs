using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FormUpDate0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //((MainWindow)System.Windows.Application.Current.MainWindow).UpdateLayout();

            if (Directory.Exists(@"c:\ARM_STAT\"))
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,new Action(() =>
                {
                    lblSync.Visibility = Visibility.Visible;
                    progressBar.Visibility = Visibility.Visible;
                    ((MainWindow)System.Windows.Application.Current.MainWindow).UpdateLayout();
                }));

                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                const string Src_FOLDER = @"\\fsttr02\стат. отчеты\СТАТИСТИКА\FORM\"; //@"G:\172.16.252.4\STAT\FORM501"; //
                const string Dest_FOLDER = @"c:\ARM_STAT\FORM\";     //@"D:\FORM2"; //

                var targetFiles = Directory.GetFiles(Dest_FOLDER, "*", SearchOption.AllDirectories);
                var notExists = targetFiles.Where(p => !File.Exists(p.Replace(Dest_FOLDER, Src_FOLDER))).ToList();
                               
                foreach (var p in notExists)
                {
                    try
                    {
                        File.Delete(p);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("emm, something went wrong?", ex.Message); //w'll deal with it later
                    }
                }

                string[] originalFiles = Directory.GetFiles(Src_FOLDER, "*", SearchOption.AllDirectories);  // Get our files (recursive and any of them, based on the 2nd param of the Directory.GetFiles() method

 
                progressBar.Maximum = originalFiles.Length;

                Array.ForEach(originalFiles, (originalFileLocation) =>              // Dealing with a string array, so let's use the actionable Array.ForEach() with a anonymous method
                {   // Get the FileInfo for both of our files
                    progressBar.Value += 1;

                    FileInfo originalFile = new FileInfo(originalFileLocation);
                    FileInfo destFile = new FileInfo(originalFileLocation.Replace(Src_FOLDER, Dest_FOLDER));
                    // ^^ We can fill the FileInfo() constructor with files that don't exist...
                    if (destFile.Exists)    // ... because we check it here
                    {   // Logic for files that exist applied here; if the original is written later, replace the updated files...

                        if (originalFile.LastWriteTime > destFile.LastWriteTime) //intead of    originalFile.Length
                        {
                            originalFile.CopyTo(destFile.FullName, true);
                        }
                    }
                    else // ... otherwise create any missing directories and copy the folder over
                    {
                        Directory.CreateDirectory(destFile.DirectoryName);   // Does nothing on directories that already exist   
                        originalFile.CopyTo(destFile.FullName, false);      // Copy but don't over-write  
                    }
                });

                Mouse.OverrideCursor = null;
                lblSync.Text = "Формы обновлены!";
                MessageBox.Show("Готово!");
            }
            else
            {
                MessageBox.Show("Не установленна необходимая версия ARM_STAT для ввода данных, пройти по ссылке и установить согласно инструкции для вашего района", "Нет необходимой версии ARM STAT", MessageBoxButton.OK, MessageBoxImage.Information);
                System.Diagnostics.Process.Start(@"\\fsttr02\стат. отчеты\HTML2019\IV\arm\armInst");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(@"\\fsttr02\стат. отчеты\HTML2019\IV\form\"))  //@"g:\192.168.15.1\СТАТ. ОТЧЕТЫ\HTML2019\IV\form\"))          
            {
                MessageBox.Show("Сетевой диск fsttr02 в данный момент не доступен, проверьте подключение, если не удалось решить проблему - свяжитесь с тех.специалистом",
                                          "Нет связи с сервером fsttr02",MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Application.Current.Shutdown();
            }
            else
            {
                Regex rx = new Regex(@"([0-9]{2}).([0-9]{2}).([0-9]{4})");
                var webCl = new WebClient();
                string html = webCl.DownloadString(@"\\fsttr02\стат. отчеты\HTML2019\IV\form\form.html");         //@"g:\192.168.15.1\СТАТ. ОТЧЕТЫ\HTML2019\IV\form\form.html");

                tbDate.Text = rx.Match(html).ToString();  //.Match finds what we looking for and displays it
            }
        }
    }
}
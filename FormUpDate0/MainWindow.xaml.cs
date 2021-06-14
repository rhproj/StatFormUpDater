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
            if (!Directory.Exists(@"\\fsttr02\стат. отчеты\AS4.6\ARM_STAT_2020\"))
            {
                MessageBox.Show("Не удалось найти необходимую папку на сетевом диске fsttr02");
                return;
            }

            Regex rx = new Regex(@"([0-9]{2}).([0-9]{2}).([0-9]{4})");
            if (rx.IsMatch(tbDate.Text))
            {
                var webCl = new WebClient();
                string html = webCl.DownloadString(@"\\fsttr02\стат. отчеты\HTML2019\IV\form\form.html");


                string htmlUptoDate = rx.Replace(html, tbDate.Text);

                using (StreamWriter sW = new StreamWriter(@"\\fsttr02\стат. отчеты\HTML2019\IV\form\form.html", false, Encoding.Default)) 
                {
                    sW.Write(htmlUptoDate);
                }
                MessageBox.Show("Готово!");
            }
            else
            {
                MessageBox.Show("Формат даты: 11.11.1111");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           tbDate.Text = DateTime.Today.ToShortDateString();
        }
    }
}

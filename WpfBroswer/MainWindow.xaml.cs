using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

namespace WpfBroswer
{

    [ComVisible(true)]
    public class BroswerEventHanlder
    {
        public void On(params object[] i)
        {
            MessageBox.Show("on");
        }
    }
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var uri = new Uri($"file:///{AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/")}map.html");
            B.ObjectForScripting = new BroswerEventHanlder();
            B.Url = uri;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var i = B.Document.InvokeScript("zoom");
                if (i != null)
                {
                    MessageBox.Show(i.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

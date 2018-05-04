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
        }


        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri($"file:///{AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/")}map.html");
            Map.ObjectForScripting = new BroswerEventHanlder();
            Map.Source = uri;
            Map.LoadCompleted -= Map_LoadCompleted;
            Map.LoadCompleted += Map_LoadCompleted;
        }

        private void Map_LoadCompleted(object sender, NavigationEventArgs e)
        {
            Console.WriteLine(e.Uri);
        }
    }
}

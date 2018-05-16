using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var uri = new Uri($"file:///{AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/")}map2.html");
            Map.ObjectForScripting = new BroswerEventHanlder();
            Map.Source = uri;
            Map.LoadCompleted -= Map_LoadCompleted;
            Map.LoadCompleted += Map_LoadCompleted;
        }

        private void Map_LoadCompleted(object sender, NavigationEventArgs e)
        {
        }

        private void ShowVersion_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri($"file:///{AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/")}test.html");
            Map.Source = uri;
            using (var i = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true))
            {
                //Console.WriteLine();
                //i.SetValue($"{Process.GetCurrentProcess().ProcessName}.exe", 8000, RegistryValueKind.DWord);
                //i.Flush();
                Version.Text = i.GetValue(Process.GetCurrentProcess().ProcessName + ".exe")?.ToString();
            }
        }

        private void ChangeVersion_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(Version.Text, out var ver))
            {
                using (var i = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true))
                {
                    i.SetValue(Process.GetCurrentProcess().ProcessName + ".exe", ver, RegistryValueKind.DWord);
                }
            }
        }
    }
}

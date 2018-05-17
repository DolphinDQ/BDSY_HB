using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseIEVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Environment:{0}\n appdomain:{1}\n process:{2}", Environment.CurrentDirectory, AppDomain.CurrentDomain.BaseDirectory, Process.GetCurrentProcess().MainModule.FileName);
            Console.Read();
            using (var i = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true))
            {
                Console.WriteLine(i.GetValue("prevhost.exe"));
                i.SetValue($"{Process.GetCurrentProcess().ProcessName}.exe", 8000, RegistryValueKind.DWord);
                i.Flush();
            }
            Console.Read();
        }
    }
}

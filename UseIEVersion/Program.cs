using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseIEVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var i = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", RegistryKeyPermissionCheck.ReadSubTree))
            {
                Console.WriteLine(i.GetValue("prevhost.exe"));
            }
            Console.Read();
        }
    }
}

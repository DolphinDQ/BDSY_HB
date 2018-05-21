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
            if (args.Length < 2) return;
            var app = args[0];
            if (!int.TryParse(args[1], out var version)) return;
            try
            {
                using (var i = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true))
                {
                    i.SetValue(app, version, RegistryValueKind.DWord);
                    i.Flush();
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}

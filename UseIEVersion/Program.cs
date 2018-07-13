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
            var path = Environment.Is64BitOperatingSystem ?
                        @"SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION" :
                        @"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";
            if (args.Length < 2) return;
            var app = args[0];
            if (!int.TryParse(args[1], out var version)) return;
            try
            {
                using (var i = Registry.LocalMachine.OpenSubKey(path, true))
                {
                    i.SetValue(app, version, RegistryValueKind.DWord);
                    i.Flush();
                }
            }
            catch (System.Security.SecurityException)
            {
                Console.WriteLine("用管理员权限打开...");
                Process.Start(new ProcessStartInfo("useie")
                {
                    Arguments = string.Join(" ", args),
                    Verb = "runas",
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("位置异常");
                Console.Write(e);
                return;
            }
        }
    }
}

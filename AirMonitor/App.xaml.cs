using Microsoft.HockeyApp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AirMonitor
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            HockeyClient.Current.Configure("75bbb694b0fd4d9891a87f46ac28d88e");
#if DEBUG
            ((HockeyClient)HockeyClient.Current).OnHockeySDKInternalException += (sender, args) =>
            {
                if (Debugger.IsAttached) { Debugger.Break(); }
            };
#endif
            await HockeyClient.Current.SendCrashesAsync(true);
           // throw new Exception("test");
            //await HockeyClient.Current.CheckForUpdatesAsync(true, () =>
            //{
            //    if (Current.MainWindow != null) { Current.MainWindow.Close(); }
            //    return true;
            //});
        }
    }
}

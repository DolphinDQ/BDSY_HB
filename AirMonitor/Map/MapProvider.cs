using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using Caliburn.Micro;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace AirMonitor.Map
{
    class MapProvider : IMapProvider
    {
        private readonly MapEventHandler m_eventHandler;

        public MapProvider(IEventAggregator eventAggregator)
        {
            m_eventHandler = new MapEventHandler(eventAggregator);
            UseIE10();
        }

        private void UseIE10()
        {
            try
            {
                using (var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true))
                {
                    var name = Process.GetCurrentProcess().ProcessName + ".exe";
                    reg.SetValue(name, 10000, RegistryValueKind.DWord);
                }
            }
            catch (Exception e)
            {
                this.Warn("user ie 10 failed.{0}", e);
                this.Error(e);
                MessageBox.Show("设置浏览器版本失败:" + e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private WebBrowser Browser { get; set; }
        public T Invoke<T>(string methodName, params object[] args)
            => Invoke(methodName, o => JsonConvert.DeserializeObject<T>(o.ToString()), args);
        public void Invoke(string methodName, params object[] args)
            => Invoke<object>(methodName, parse: null, args: args);

        public T Invoke<T>(string methodName, Func<object, T> parse, params object[] args)
        {
            try
            {
                if (methodName.Contains("."))
                {

                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i] is string)
                        {
                            args[i] = $"'{args[i]}'";
                        }
                    }
                    var tmpArg = $"{methodName}({string.Join(",", args)})";
                    args = new[] { tmpArg };
                    methodName = "eval";
                }
                return Browser.Dispatcher.Invoke(() =>
                 {
                     var obj = Browser.InvokeScript(methodName, args);
                     return obj == null || parse == null ? default(T) : parse(obj);
                 });
            }
            catch (Exception e)
            {
                this.Warn("invoke js [{0}] error.", methodName);
                this.Error(e);
                return default(T);
            }

        }

        public void LoadMap(object mapContainer)
        {
            if (mapContainer is WebBrowser)
            {
                Browser = mapContainer as WebBrowser;
                Browser.Dispatcher.Invoke(() =>
                {
                    Browser.ObjectForScripting = m_eventHandler;
                    Browser.Source = new Uri($"file:///{AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/")}Map/map.html");
                });
            }
        }


    }
}

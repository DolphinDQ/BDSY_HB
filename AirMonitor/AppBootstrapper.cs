namespace AirMonitor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Threading;
    using AirMonitor.Camera;
    using AirMonitor.Chart;
    using AirMonitor.Config;
    using AirMonitor.Core;
    using AirMonitor.Data;
    using AirMonitor.Interfaces;
    using AirMonitor.Map;
    using AirMonitor.ViewModels;
    using Caliburn.Micro;
    using System.Linq;

    public class AppBootstrapper : BootstrapperBase, IFactory
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        static SimpleContainer container;

        public AppBootstrapper()
        {
            var process = Process.GetCurrentProcess();
            var arr = Process.GetProcessesByName(process.ProcessName);
            if (arr.Length > 1)
            {
                var hanler = arr.FirstOrDefault(o => o.Id != process.Id)?.MainWindowHandle;
                if (hanler != null)
                {
                    Console.WriteLine(ShowWindow(hanler.Value, 1));
                    Console.WriteLine(SetForegroundWindow(hanler.Value));
                    Application.Current.Shutdown();
                    return;
                }
            }
            Initialize();
        }

        protected override void Configure()
        {
            container = new SimpleContainer();
            container.Singleton<IWindowManager, WindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();
            container.Singleton<IResourceManager, ResourceProvider>();
            container.Singleton<ILog, Logger>();
            container.Singleton<ISaveManager, SaveManager>();
            container.Singleton<IDataManager, MqttDataManager>();
            container.Singleton<IConfigManager, ConfigManager>();
            container.Singleton<IChartManager, ChartManager>();
            container.Singleton<ICameraManager, BVCUCameraManager>();
            container.Singleton<IBackupManager, BackupManager>();
            container.PerRequest<IShell, ShellViewModel>();
            container.PerRequest<IMapProvider, MapProvider>();
            container.RegisterInstance(typeof(IFactory), null, this);
            container.PerRequest<DataPushViewModel>();
            container.PerRequest<MapViewModel>();
            container.PerRequest<ConfigAirPollutantViewModel>();
            container.PerRequest<AnalysisStaticViewModel>();
            container.PerRequest<AnalysisDynamicViewModel>();
            container.PerRequest<ConfigPushServerViewModel>();
            container.PerRequest<ConfigCameraViewModel>();
            container.PerRequest<Map3DViewModel>();
            container.Singleton<SimulatorViewModel>();
            container.Singleton<SaveSampleViewModel>();
            container.PerRequest<VideoViewModel>();
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            this.Warn("unhandle error from {0} : {1}", sender, e.Exception.Message);
            this.Error(e.Exception);
            e.Handled = true;
        }


        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            DisplayRootViewFor<IShell>();
        }

        public T Create<T>(string name = null)
        {
            return container.GetInstance<T>(name);
        }
    }

}
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

        public static SimpleContainer Container { get; private set; }

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
            Container = new SimpleContainer();
            Container.Singleton<IWindowManager, WindowManager>();
            Container.Singleton<IEventAggregator, EventAggregator>();
            Container.Singleton<IResourceManager, ResourceProvider>();
            Container.Singleton<ILog, Logger>();
            Container.Singleton<ISaveManager, SaveManager>();
            Container.Singleton<IDataManager, MqttDataManager>();
            Container.Singleton<IConfigManager, ConfigManager>();
            Container.Singleton<IChartManager, ChartManager>();
            Container.Singleton<ICameraManager, BVCUCameraManager>();
            Container.Singleton<IBackupManager, BackupManager>();
            Container.PerRequest<IShell, ShellViewModel>();
            Container.PerRequest<IMapProvider, MapProvider>();
            Container.RegisterInstance(typeof(IFactory), null, this);
            Container.PerRequest<DataPushViewModel>();
            Container.PerRequest<MapViewModel>();
            Container.PerRequest<ConfigAirPollutantViewModel>();
            Container.PerRequest<AnalysisStaticViewModel>();
            Container.PerRequest<AnalysisDynamicViewModel>();
            Container.PerRequest<ConfigPushServerViewModel>();
            Container.PerRequest<ConfigCameraViewModel>();
            Container.PerRequest<Map3DViewModel>();
            Container.Singleton<SimulatorViewModel>();
            Container.Singleton<SaveSampleViewModel>();
            Container.PerRequest<VideoViewModel>();
            Container.PerRequest<AirSampleDisplayViewModel>();
            Container.PerRequest<PollutantViewModel>();
            Container.PerRequest<UavStatusDisplayViewModel>();
            Container.PerRequest<SiderViewModel>();
            
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            this.Warn("unhandle error from {0} : {1}", sender, e.Exception.Message);
            this.Error(e.Exception);
            e.Handled = true;
        }


        protected override object GetInstance(Type service, string key)
        {
            return Container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return Container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            Container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            DisplayRootViewFor<IShell>();
        }

        public T Create<T>(string name = null)
        {
            return Container.GetInstance<T>(name);
        }
    }

}
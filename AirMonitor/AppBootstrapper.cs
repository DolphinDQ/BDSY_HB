namespace AirMonitor
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Threading;
    using AirMonitor.Config;
    using AirMonitor.Core;
    using AirMonitor.Data;
    using AirMonitor.Interfaces;
    using AirMonitor.Map;
    using AirMonitor.ViewModels;
    using Caliburn.Micro;

    public class AppBootstrapper : BootstrapperBase, IFactory
    {
        static SimpleContainer container;

        public AppBootstrapper()
        {
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
            container.PerRequest<IShell, ShellViewModel>();
            container.PerRequest<IMapProvider, MapProvider>();
            container.RegisterInstance(typeof(IFactory), null, this);
            container.PerRequest<DataDisplayViewModel>();
            container.PerRequest<MapViewModel>();
            container.PerRequest<PollutantSettingViewModel>();
            
            LogManager.GetLog = o => container.GetInstance<ILog>();
            container.GetInstance<IDataManager>().Init();
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            this.Warn("unhandle error  from {0} : {1}", sender, e.Exception.Message);
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

    public static class GlobalLogger
    {
        public static void Error(this object obj, Exception exception) => LogManager.GetLog(typeof(ILog)).Error(exception);

        public static void Info(this object obj, string format, params object[] args) => LogManager.GetLog(typeof(ILog)).Info($"[{obj.GetType().Name}]" + format, args);

        public static void Warn(this object obj, string format, params object[] args) => LogManager.GetLog(typeof(ILog)).Warn($"[{obj.GetType().Name}]" + format, args);
    }
}
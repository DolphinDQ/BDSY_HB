namespace AirMonitor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Threading;
    using AirMonitor.Camera;
    using AirMonitor.Chart;
    using AirMonitor.Config;
    using AirMonitor.Core;
    using AirMonitor.Interfaces;
    using AirMonitor.Map;
    using AirMonitor.ViewModels;
    using Caliburn.Micro;
    using Microsoft.HockeyApp;

    public class AppBootstrapper : BootstrapperBase, IFactory
    {
        public static SimpleContainer Container { get; private set; }

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            Container = new SimpleContainer();
            Container.Singleton<IWindowManager, WindowManager>();
            Container.Singleton<IEventAggregator, EventAggregator>();
            Container.Singleton<IResourceManager, ResourceProvider>();
            Container.Singleton<Caliburn.Micro.ILog, Logger>();
            Container.Singleton<ISaveManager, SaveManager>();
            Container.Singleton<IConfigManager, ConfigManager>();
            Container.Singleton<IChartManager, ChartManager>();
            Container.PerRequest<IShell, ShellViewModel>();
            Container.PerRequest<IMapProvider, MapProvider>();
            Container.RegisterInstance(typeof(IFactory), null, this);
            Container.PerRequest<MapViewModel>();
            Container.PerRequest<LoginViewModel>();

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
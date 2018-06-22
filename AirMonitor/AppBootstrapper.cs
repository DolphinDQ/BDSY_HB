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
    using AirMonitor.Data;
    using AirMonitor.Interfaces;
    using AirMonitor.Map;
    using AirMonitor.ViewModels;
    using Caliburn.Micro;
    using Microsoft.HockeyApp;

    public class AppBootstrapper : BootstrapperBase, IFactory
    {
        static SimpleContainer container;

        public AppBootstrapper()
        {
            Initialize();
        }



        protected override  void Configure()
        {

            container = new SimpleContainer();
            container.Singleton<IWindowManager, WindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();
            container.Singleton<IResourceManager, ResourceProvider>();
            container.Singleton<Caliburn.Micro.ILog, Logger>();
            container.Singleton<ISaveManager, SaveManager>();
            container.Singleton<IDataManager, MqttDataManager>();
            container.Singleton<IConfigManager, ConfigManager>();
            container.Singleton<IChartManager, ChartManager>();
            container.Singleton<ICameraManager, BVCUCameraManager>();
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
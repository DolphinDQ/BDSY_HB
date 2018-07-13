namespace AirMonitor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Input;
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
        public static SimpleContainer Container { get; private set; }

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {

            var defaultCreateTrigger = Parser.CreateTrigger;

            Parser.CreateTrigger = (target, triggerText) =>
            {
                if (triggerText == null)
                {
                    return defaultCreateTrigger(target, null);
                }

                var triggerDetail = triggerText
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty);

                var splits = triggerDetail.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

                switch (splits[0])
                {
                    case "Key":
                        var key = (Key)Enum.Parse(typeof(Key), splits[1], true);
                        return new KeyTrigger { Key = key };

                    //case "Gesture":
                    //    var mkg = (MultiKeyGesture)(new MultiKeyGestureConverter()).ConvertFrom(splits[1]);
                    //    return new KeyTrigger { Modifiers = mkg.KeySequences[0].Modifiers, Key = mkg.KeySequences[0].Keys[0] };
                }

                return defaultCreateTrigger(target, triggerText);
            };
            Container = new SimpleContainer();
            Container.Singleton<IWindowManager, WindowManager>();
            Container.Singleton<IEventAggregator, EventAggregator>();
            Container.Singleton<IResourceManager, ResourceProvider>();
            Container.Singleton<Caliburn.Micro.ILog, Logger>();
            Container.Singleton<ISaveManager, SaveManager>();
            Container.Singleton<IConfigManager, ConfigManager>();
            Container.Singleton<IChartManager, ChartManager>();
            Container.Singleton<IDataQueryManager, QueryManager>();
            Container.PerRequest<IShell, ShellViewModel>();
            Container.PerRequest<IMapProvider, MapProvider>();
            Container.RegisterInstance(typeof(IFactory), null, this);
            Container.PerRequest<MapViewModel>();
            Container.PerRequest<LoginViewModel>();
            Container.Singleton<SaveSampleViewModel>();
            Container.PerRequest<AnalysisStaticViewModel>(); 
            Container.PerRequest<Map3DViewModel>(); 
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
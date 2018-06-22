using AirMonitor.Config;
using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using AirMonitor.ViewModels;
using Caliburn.Micro;
using Microsoft.HockeyApp;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace AirMonitor
{
    public class ShellViewModel : Screen, IShell,
        IHandle<EvtSetting>,
        IHandle<EvtSampleSaving>,
        IHandle<EvtMapSavePoints>
    {
        private IFactory m_factory;
        private IResourceManager m_res;
        private ISaveManager m_saveManager;
        private IEventAggregator m_eventAggregator;

        public ShellViewModel(
            IFactory factory,
            ISaveManager saveManager,
            IDataManager dataManager,
            IEventAggregator eventAggregator,
            IResourceManager res)
        {
            m_factory = factory;
            m_res = res;
            m_saveManager = saveManager;
            m_eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
            LogManager.GetLog = o => factory.Create<Caliburn.Micro.ILog>();
            dataManager.Init();
            DisplayName = m_res.GetText("T_AppName") + " - " + AppVersion.VERSION;
        }

        public object Sider { get; set; }

        public object Container { get; set; }

        public object Setting { get; set; }

        public bool EnableSetting { get; set; }

        public string SettingTitle { get; set; }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            HockeyClient.Current.Configure("75bbb694b0fd4d9891a87f46ac28d88e");
#if DEBUG
            ((HockeyClient)HockeyClient.Current).OnHockeySDKInternalException += (sender, args) =>
            {
                if (Debugger.IsAttached) { Debugger.Break(); }
            };
#endif
            await HockeyClient.Current.SendCrashesAsync(true);
        }


        public override void TryClose(bool? dialogResult = null)
        {
            base.TryClose(dialogResult);
            m_eventAggregator.Unsubscribe(this);
        }

        public void Handle(EvtSetting message)
        {
            switch (message.Command)
            {
                case SettingCommands.Request:
                    if (EnableSetting) return;
                    CloseSetting();
                    var obj = message.SettingObject;
                    if (obj is AirStandardSetting airStandard)
                    {
                        OpenAirStandardSetting(airStandard);
                    }
                    else if (obj is CameraSetting camera)
                    {
                        OpenCameraSetting(camera);
                    }
                    else if (obj is MqttSetting mqtt)
                    {
                        OpenMqttSetting(mqtt);
                    }
                    break;
                case SettingCommands.Changed:
                default:
                    Setting = null;
                    EnableSetting = false;
                    break;
            }
        }

        private void OpenMqttSetting(MqttSetting mqtt)
        {
            var setting = m_factory.Create<ConfigPushServerViewModel>();
            setting.Setting = mqtt;
            SettingTitle = m_res.GetText("T_DataServiceSetting");
            Setting = setting;
            EnableSetting = true;
        }

        private void CloseSetting()
        {
            if (Setting is Screen screen)
            {
                screen.TryClose();
            }
        }

        private void OpenAirStandardSetting(AirStandardSetting airStandard)
        {
            var setting = m_factory.Create<ConfigAirPollutantViewModel>();
            setting.Settings = airStandard;
            SettingTitle = m_res.GetText("T_SampleSetting");
            Setting = setting;
            EnableSetting = true;
        }

        private void OpenCameraSetting(CameraSetting camera)
        {
            var setting = m_factory.Create<ConfigCameraViewModel>();
            setting.Setting = camera;
            SettingTitle = m_res.GetText("T_CameraSetting");
            Setting = setting;
            EnableSetting = true;
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            Sider = m_factory.Create<DataPushViewModel>();
            Container = m_factory.Create<MapViewModel>();
        }

        public void OpenSimulator()
        {
            CloseSetting();
            Setting = m_factory.Create<SimulatorViewModel>();
            SettingTitle = m_res.GetText("T_Simulation");
            EnableSetting = true;
        }

        public void ClearAllSample()
        {
            if (MessageBox.Show(m_res.GetText("T_ClearSamplesWarning"), "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                m_eventAggregator.PublishOnBackgroundThread(new EvtSampling() { Status = SamplingStatus.ClearAll });
            }
        }

        public void SaveSamples()
        {
            m_eventAggregator.PublishOnBackgroundThread(new EvtSampleSaving() { Type = SaveType.SaveSamplesRequest });
        }

        public void LoadSamples()
        {
            m_eventAggregator.PublishOnBackgroundThread(new EvtSampleSaving() { Type = SaveType.LoadSamplesRequest });
        }

        public void Handle(EvtSampleSaving message)
        {
            if (message.Type == SaveType.SaveSamples &&
                (message.Save == null || message.Save.Samples == null || !message.Save.Samples.Any()))
            {
                return;//如果保存数据为空则不处理。
            }
            CloseSetting();
            switch (message.Type)
            {
                case SaveType.SaveSamples:
                case SaveType.LoadSamples:
                    var model = m_factory.Create<SaveSampleViewModel>();
                    model.Evt = message;
                    Setting = model;
                    SettingTitle = m_res.GetText(message.Type);
                    EnableSetting = true;
                    break;
                default:
                    EnableSetting = false;
                    break;
            }

        }

        public void Handle(EvtMapSavePoints message)
        {
            Handle(new EvtSampleSaving()
            {
                Save = new AirSamplesSave()
                {
                    Samples = message.points.Cast<EvtAirSample>().ToArray()
                },
                Type = SaveType.SaveSamples
            });
        }
    }
}
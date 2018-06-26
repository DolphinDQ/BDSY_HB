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
            IEventAggregator eventAggregator,
            IResourceManager res)
        {
            m_factory = factory;
            m_res = res;
            m_saveManager = saveManager;
            m_eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
            LogManager.GetLog = o => factory.Create<Caliburn.Micro.ILog>();
            DisplayName = m_res.GetText("T_ViewerName") + " - " + AppVersion.VERSION;
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

     
        private void CloseSetting()
        {
            if (Setting is Screen screen)
            {
                screen.TryClose();
            }
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
            //if (message.Type == SaveType.SaveSamples &&
            //    (message.Save == null || message.Save.Samples == null || !message.Save.Samples.Any()))
            //{
            //    return;//如果保存数据为空则不处理。
            //}
            //CloseSetting();
            //switch (message.Type)
            //{
            //    case SaveType.SaveSamples:
            //    case SaveType.LoadSamples:
            //        var model = m_factory.Create<SaveSampleViewModel>();
            //        model.Evt = message;
            //        Setting = model;
            //        SettingTitle = m_res.GetText(message.Type);
            //        EnableSetting = true;
            //        break;
            //    default:
            //        EnableSetting = false;
            //        break;
            //}
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
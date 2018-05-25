using AirMonitor.Config;
using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using AirMonitor.ViewModels;
using Caliburn.Micro;
using System.Windows;

namespace AirMonitor
{
    public class ShellViewModel : Screen, IShell, IHandle<EvtSetting>
    {
        private IFactory m_factory;
        private IResourceManager m_res;
        private IEventAggregator m_eventAggregator;

        public ShellViewModel(IFactory factory, IDataManager dataManager, IEventAggregator eventAggregator, IResourceManager res)
        {
            m_factory = factory;
            m_res = res;
            m_eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
            LogManager.GetLog = o => factory.Create<ILog>();
            dataManager.Init();
        }

        public object Sider { get; set; }

        public object Container { get; set; }

        public object Setting { get; set; }

        public bool EnableSetting { get; set; }

        public string SettingTitle { get; set; }

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
                    var obj = message.SettingObject;
                    if (obj is AirStandardSetting setting)
                    {
                        OpenSetting();
                    }
                    break;
                case SettingCommands.Changed:
                default:
                    Setting = null;
                    EnableSetting = false;
                    break;
            }
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            Sider = m_factory.Create<DataDisplayViewModel>();
            Container = m_factory.Create<MapViewModel>();
        }

        public void OpenSetting()
        {
            Setting = m_factory.Create<PollutantSettingViewModel>();
            SettingTitle = m_res.GetText("T_Setting");
            EnableSetting = true;
        }

        public void OpenSimulator()
        {
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
    }
}
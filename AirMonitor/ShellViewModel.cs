using AirMonitor.Config;
using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using AirMonitor.ViewModels;
using Caliburn.Micro;

namespace AirMonitor
{
    public class ShellViewModel : Screen, IShell, IHandle<EvtSetting>
    {
        private IFactory m_factory;
        private IEventAggregator m_eventAggregator;

        public ShellViewModel(IFactory factory, IEventAggregator eventAggregator)
        {
            m_factory = factory;
            m_eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
        }

        public object Sider { get; set; }

        public object Container { get; set; }

        public object Setting { get; set; }

        public bool EnableSetting { get; set; }

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
                        var tmp = m_factory.Create<PollutantSettingViewModel>();
                        tmp.Settings = setting;
                        Setting = tmp;
                    }
                    EnableSetting = true;
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

    }
}
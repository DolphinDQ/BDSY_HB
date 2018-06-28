using AirMonitor.Config;
using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.ViewModels
{
    public class AirSampleDisplayViewModel : Screen,
        IHandle<EvtAirSample>,
        IHandle<EvtSampling>,
        IHandle<EvtSetting>
    {
        private IEventAggregator m_eventAggregator;
        private IConfigManager m_configManager;
        private IFactory m_factory;

        public AirSampleDisplayViewModel(IFactory factory,
            IConfigManager configManager,
            IEventAggregator eventAggregator)
        {
            m_eventAggregator = eventAggregator;
            m_configManager = configManager;
            m_factory = factory;
            PollutantViewList = new ObservableCollection<PollutantViewModel>();
            m_eventAggregator.Subscribe(this);
        }
        public ObservableCollection<PollutantViewModel> PollutantViewList { get; private set; }
        public bool Sampling { get; set; }

        public void OnSamplingChanged()
        {
            m_eventAggregator.PublishOnBackgroundThread(new EvtSampling() { Status = Sampling ? SamplingStatus.Start : SamplingStatus.Stop });
        }

        public void Handle(EvtAirSample message)
        {
            if (Sampling)
            {
                foreach (var item in PollutantViewList)
                {
                    item.Add(message);
                }
            }
        }

        public void Handle(EvtSetting message)
        {
            if (message.Command != SettingCommands.Changed || !PollutantViewList.Any()) return;
            if (message.SettingObject is AirStandardSetting setting)
            {
                foreach (var item in PollutantViewList)
                {
                    if (item.Standard != null)
                    {
                        var standard = setting.Pollutant.FirstOrDefault(o => o.Name == item.Standard.Name);
                        if (standard != null)
                        {
                            item.Standard = standard;
                        }
                    }
                }
            }
        }

        public void Handle(EvtSampling message)
        {
            switch (message.Status)
            {
                case SamplingStatus.Stop:
                    Sampling = false;
                    break;
                case SamplingStatus.Start:
                    Sampling = true;
                    break;
                case SamplingStatus.ClearAll:
                    foreach (var item in PollutantViewList)
                    {
                        item.Clear();
                    }
                    break;
            }
        }

        public override void TryClose(bool? dialogResult = null)
        {
            base.TryClose(dialogResult);
            foreach (var item in PollutantViewList)
            {
                item.TryClose();
            }
            m_eventAggregator.Unsubscribe(this);
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            var setting = m_configManager.GetConfig<AirStandardSetting>();
            foreach (var item in setting.Pollutant)
            {
                var model = m_factory.Create<PollutantViewModel>();
                model.Standard = item;
                PollutantViewList.Add(model);
            }
        }


    }
}

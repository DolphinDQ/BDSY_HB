using AirMonitor.Config;
using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using Caliburn.Micro;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.ViewModels
{
    class PollutantSettingViewModel : Screen
    {
        private IConfigManager m_configManager;
        private IEventAggregator m_eventAggregator;

        public PollutantSettingViewModel(IConfigManager configManager, IEventAggregator eventAggregator)
        {
            m_configManager = configManager;
            m_eventAggregator = eventAggregator;
        }

        public AirStandardSetting Settings { get; set; }

        [DependsOn(nameof(Settings))]
        public AirPollutant[] Pollutants => Settings?.Pollutant;

        public AirPollutant Current { get; set; }

        public void Confirm()
        {
            if (Settings != null)
            {
                m_configManager.SaveConfig(Settings);
                m_eventAggregator.PublishOnBackgroundThread(new EvtSetting() { Command = SettingCommands.Changed, SettingObject = Settings });
            }
        }

        public void OnSettingsChanged()
        {
            if (Settings != null)
            {
                Current = Settings.Pollutant.FirstOrDefault();
            }
        }
    }
}

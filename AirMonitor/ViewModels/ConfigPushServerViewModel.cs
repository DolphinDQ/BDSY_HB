using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirMonitor.Config;
using AirMonitor.Interfaces;

namespace AirMonitor.ViewModels
{
    class ConfigPushServerViewModel
    {
        private IConfigManager m_configManager;

        public ConfigPushServerViewModel(IConfigManager configManager)
        {
            m_configManager = configManager;
        }
        public MqttSetting Setting { get; set; }

        public void Confirm()
        {
            m_configManager.SaveConfig(Setting);
        }
    }
}

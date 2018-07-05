using AirMonitor.Config;
using AirMonitor.Interfaces;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AirMonitor.ViewModels
{
    class LoginViewModel : Screen
    {
        private IConfigManager m_config;
        private IWindowManager m_window;

        public bool IsRemember { get; set; }

        public FtpSetting Setting { get; private set; }

        public LoginViewModel(IConfigManager config, IWindowManager window)
        {
            DisplayName = "请登录用户";
            m_config = config;
            m_window = window;
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            Setting = m_config.GetConfig<FtpSetting>();
            if (!Setting.RememberPassword)
            {
                Setting.Password = null;
            }
        }

        public void Login()
        {
            m_config.SaveConfig(Setting);
            TryClose(true);
        }

    }
}

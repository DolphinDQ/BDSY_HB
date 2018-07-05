using AirMonitor.Config;
using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using AirMonitor.ViewModels;
using Caliburn.Micro;
using FluentFTP;
using Microsoft.HockeyApp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;

namespace AirMonitor
{
    public class ShellViewModel : Screen, IShell,
        IHandle<EvtSampleSaving>,
        IHandle<EvtMapSavePoints>
    {
        private IFactory m_factory;
        private IResourceManager m_res;
        private IConfigManager m_config;
        private IWindowManager m_window;
        private ISaveManager m_saveManager;
        private IEventAggregator m_eventAggregator;

        public ShellViewModel(
            IFactory factory,
            IConfigManager config,
            IWindowManager window,
            ISaveManager saveManager,
            IEventAggregator eventAggregator,
            IResourceManager res)
        {
            m_factory = factory;
            m_res = res;
            m_config = config;
            m_window = window;
            m_saveManager = saveManager;
            m_eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
            LogManager.GetLog = o => factory.Create<Caliburn.Micro.ILog>();
            DisplayName = m_res.GetText("T_ViewerName") + " - " + AppVersion.VERSION;

            //MainContent = m_factory.Create<MapViewModel>();
        }
        public object MainContent { get; set; }

        #region Sider
        public object Sider { get; set; }

        public bool ShowSider { get; set; }

        public string SiderTitle { get; set; }

        public async void OnShowSiderChanged()
        {
            if (!ShowSider)
            {
                if (Sider is Screen screen)
                {
                    screen.TryClose();
                }
                await Task.Delay(300);
                Sider = null;
            }
        }

        public void OnSiderChanged()
        {
            if (!ShowSider && Sider != null)
            {
                ShowSider = true;
            }
        }
        /// <summary>
        /// 侧边栏显示和隐藏。
        /// </summary>
        /// <param name="model">要显示的内容，如果为null则隐藏侧边栏。</param>
        private void SiderDisplay(object model)
        {
            ShowSider = false;
            if (model != null)
            {
                Sider = model;
            }
        }
        #endregion

        public string LoginUser { get; set; }

        protected override async void OnViewAttached(object view, object context)
        {
            HockeyClient.Current.Configure("75bbb694b0fd4d9891a87f46ac28d88e");
#if DEBUG
            ((HockeyClient)HockeyClient.Current).OnHockeySDKInternalException += (sender, args) =>
            {
                if (Debugger.IsAttached) { Debugger.Break(); }
            };
#endif
            await HockeyClient.Current.SendCrashesAsync(true);
            await ShowLoginDialog();
            base.OnViewAttached(view, context);
        }

        private async Task ShowLoginDialog()
        {
            var dir = new Dictionary<string, object>();
            dir.Add("ResizeMode", 0);
            if (m_window.ShowDialog(m_factory.Create<LoginViewModel>(), null, dir) == true)
            {
                await OnLogin();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private async Task OnLogin()
        {
            var setting = m_config.GetConfig<FtpSetting>();
            try
            {
                var provier = new FtpProvider(setting);
                await provier.Ftp.ConnectAsync();
                LoginUser = setting.Account;
                OnLoginCompleted();
                return;
            }
            catch (FtpCommandException e) when (e.CompletionCode == "530")
            {
                MessageBox.Show(m_res.GetText("T_IncorrentLogin"));
                await ShowLoginDialog();
            }
            catch (Exception e)
            {
                this.Warn("connect ftp error :{0}", e.Message);
                this.Error(e);
            }
        }

        private void OnLoginCompleted()
        {
            MainContent = m_factory.Create<MapViewModel>();

        }

        public override void TryClose(bool? dialogResult = null)
        {
            base.TryClose(dialogResult);
            m_eventAggregator.Unsubscribe(this);
        }

        private void CloseSetting()
        {
            if (Sider is Screen screen)
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
                    SiderDisplay(model);
                    SiderTitle = m_res.GetText(message.Type);
                    break;
                default:
                    SiderDisplay(null);
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
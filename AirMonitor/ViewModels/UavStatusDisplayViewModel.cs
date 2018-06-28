using AirMonitor.Config;
using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.ViewModels
{
    class UavStatusDisplayViewModel : Screen,
        IHandle<EvtCameraGetDevices>,
        IHandle<EvtAirSample>
    {
        public UavStatusDisplayViewModel(IDataManager data,
            IEventAggregator eventAggregator,
            IFactory factory,
            IWindowManager windowManager,
            IConfigManager configManager,
            ICameraManager camera)
        {
            DataManager = data;
            CameraManager = camera;
            m_factory = factory;
            m_windowManager = windowManager;
            m_eventAggregator = eventAggregator;
            m_configManager = configManager;
            CameraSetting = configManager.GetConfig<CameraSetting>();
            PollutantSetting = configManager.GetConfig<AirStandardSetting>();
            m_eventAggregator.Subscribe(this);
        }

        public override void TryClose(bool? dialogResult = null)
        {
            base.TryClose(dialogResult);
            m_eventAggregator.Unsubscribe(this);
        }

        public CameraSetting CameraSetting { get; private set; }
        public AirStandardSetting PollutantSetting { get; private set; }
        public IDataManager DataManager { get; }
        public ICameraManager CameraManager { get; }

        private IFactory m_factory;
        private IWindowManager m_windowManager;
        private IEventAggregator m_eventAggregator;
        private IConfigManager m_configManager;
        public bool IsCameraOnline { get; set; }
        public EvtAirSample Current { get; set; }
        public void Handle(EvtCameraGetDevices message)
        {
            if (CameraSetting != null)
            {
                var cam = message.Devices.FirstOrDefault(o => o.Id == CameraSetting.CameraId);
                if (cam != null)
                {
                    var chnl = cam.Channel.FirstOrDefault(o => o.Channel == CameraSetting.ChannelIndex);
                    if (chnl != null)
                    {
                        IsCameraOnline = chnl.IsOnline;
                        return;
                    }
                }
                m_eventAggregator.PublishOnBackgroundThread(new EvtSetting() { Command = SettingCommands.Request, SettingObject = CameraSetting });
            }
        }

        public void Handle(EvtAirSample message)
        {
            Current = message;
        }

        public void OpenVideoDialog()
        {
            var view = m_factory.Create<VideoViewModel>();
            var dir = new Dictionary<string, object>();
            //dir.Add("WindowState", 2);
            //dir.Add("Width", 1280);
            //dir.Add("Heght", 720);
            m_windowManager.ShowWindow(view, null, dir);
        }
        public void VideoServiceSetting()
        {
            m_eventAggregator.PublishOnBackgroundThread(new EvtSetting() { Command = SettingCommands.Request, SettingObject = CameraSetting });
        }

        public void DataServiceSetting()
        {
            m_eventAggregator.PublishOnBackgroundThread(new EvtSetting() { Command = SettingCommands.Request, SettingObject = m_configManager.GetConfig<MqttSetting>() });
        }

        public void SampleSetting()
        {
            m_eventAggregator.PublishOnBackgroundThread(new EvtSetting() { Command = SettingCommands.Request, SettingObject = PollutantSetting });
        }


    }
}

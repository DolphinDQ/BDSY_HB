using AirMonitor.Camera;
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
    class ConfigCameraViewModel : Screen, IHandle<EvtCameraGetDevices>
    {
        private IConfigManager m_configManager;
        private ICameraManager m_cameraManager;
        private IEventAggregator m_eventAggregator;

        public ConfigCameraViewModel(
            IConfigManager configManager,
            IEventAggregator eventAggregator,
            ICameraManager cameraManager)
        {
            m_configManager = configManager;
            m_cameraManager = cameraManager;
            m_eventAggregator = eventAggregator;
            m_eventAggregator.Subscribe(this);
        }
        public override void TryClose(bool? dialogResult = null)
        {
            base.TryClose(dialogResult);
            m_eventAggregator.Unsubscribe(this);
        }

        public CameraSetting Setting { get; set; }

        public IEnumerable<CameraDevice> Devices { get; set; }

        public CameraDevice SelectedCamera { get; set; }

        [PropertyChanged.DependsOn(nameof(SelectedCamera))]
        public string VideoChannel
        {
            get
            {
                var i = SelectedCamera.Channel?.FirstOrDefault();
                return i == null ? "" : $"{i.Channel}#{i.Name}";
            }
        }

        public void OnSelectedCameraChanged()
        {
            if (SelectedCamera != null)
            {
                Setting.CameraId = SelectedCamera.Id;
                Setting.VideoChanel = SelectedCamera.Channel?.FirstOrDefault()?.Channel ?? 0;
                NotifyOfPropertyChange(nameof(Setting));
            }
        }

        public void OnDevicesChanged()
        {
            if (Devices != null)
            {
                var device = Devices.FirstOrDefault(o => o.Id == Setting.CameraId);
                if (device != null)
                {
                    SelectedCamera = device;
                }
            }
        }

        public void Handle(EvtCameraGetDevices message)
        {
            Devices = message.Devices;
        }

        public void RefreshList()
        {
            m_cameraManager.GetDevices();
        }

        public void Confirm()
        {
            if (Setting != null)
            {
                m_configManager.SaveConfig(Setting);
            }
        }

    }
}

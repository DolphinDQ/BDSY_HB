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
    class ConfigCameraViewModel : Screen, IHandle<EvtCameraGetDevices>, IHandle<EvtCameraConnect>
    {
        private IConfigManager m_configManager;
        private IEventAggregator m_eventAggregator;

        public ConfigCameraViewModel(
            IConfigManager configManager,
            IEventAggregator eventAggregator,
            ICameraManager cameraManager)
        {
            m_configManager = configManager;
            CameraManager = cameraManager;
            m_eventAggregator = eventAggregator;
            m_eventAggregator.Subscribe(this);
        }
        public override void TryClose(bool? dialogResult = null)
        {
            base.TryClose(dialogResult);
            m_eventAggregator.Unsubscribe(this);
        }
        public ICameraManager CameraManager { get; }

        public CameraSetting Setting { get; set; }

        public IEnumerable<CameraDevice> Devices { get; set; }

        public CameraDevice SelectedCamera { get; set; }

        public void OnSelectedCameraChanged()
        {
            if (SelectedCamera != null)
            {
                Setting.CameraId = SelectedCamera.Id;
                Setting.ChannelIndex = SelectedCamera.Channel?.FirstOrDefault()?.Channel ?? 0;
                Setting.ChannelName = SelectedCamera.Channel?.FirstOrDefault()?.Name ?? "";
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
            CameraManager.GetDevices();
        }

        public void Confirm()
        {
            if (Setting != null)
            {
                m_configManager.SaveConfig(Setting);
            }
        }

        public void Reconnect()
        {
            CameraManager.Reconnect();
        }

        public void Handle(EvtCameraConnect message)
        {
            NotifyOfPropertyChange(nameof(CameraManager));
        }
    }
}

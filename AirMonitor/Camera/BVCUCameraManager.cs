using AirMonitor.Config;
using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using BVCUSDK;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AirMonitor.Camera
{
    public class BVCUCameraManager : ICameraManager
    {
        private readonly BVCU_Cmd_OnGetPuList m_getPuList;
        private readonly BVCU_Server_OnEvent m_serverEvent;
        private readonly BVCU_Server_ProcChannelInfo m_serverProcChannelInfo;
        private readonly BVCU_Dialog_OnDialogEvent m_dialog_OnDialogEvent;
        private readonly BVCU_Dialog_OnStorageEvent m_dialog_OnStorageEvent;
        private readonly BVCU.DisplayFont m_afterRenderDisplayFont;
        private readonly IEventAggregator m_eventAggregator;
        private IntPtr m_sdkHandle;
        private IntPtr m_sessionHandler;
        private IntPtr m_dialogHandle;
        private List<CameraDevice> m_devices;
        private bool m_gettingDevice;

        private IConfigManager m_configManager;

        public CameraSetting Setting { get; private set; }

        public bool IsConnected { get; private set; }

        public BVCUCameraManager(IConfigManager configManager, IEventAggregator eventAggregator)
        {
            BVCU.FAILED(BVCU.ManagedLayer_CuInit(ref m_sdkHandle));
            m_eventAggregator = eventAggregator;
            m_configManager = configManager;
            m_serverEvent = new BVCU_Server_OnEvent(OnServerEvent);
            m_serverProcChannelInfo = new BVCU_Server_ProcChannelInfo(OnProcChannelInfo);
            m_getPuList = new BVCU_Cmd_OnGetPuList(OnGetPuList);
            m_dialog_OnDialogEvent = new BVCU_Dialog_OnDialogEvent(OnDialogEvent);
            m_dialog_OnStorageEvent = new BVCU_Dialog_OnStorageEvent(OnStorageEvent);
            m_afterRenderDisplayFont = new BVCU.DisplayFont(OnRander);
            BVCU.ManagedLayer_DispSetDisplayFontFunc(m_afterRenderDisplayFont);//设置渲染回调，不然会报错。
            Setting = m_configManager.GetConfig<CameraSetting>();
            Reconnect();
        }

        private void OnRander(IntPtr dialog, long timeStamp)
        {

        }

        private void OnStorageEvent(IntPtr dialog, int eventCode, BVCU.BVCU_Result errorCode, IntPtr fileName, int strLen, long timeStamp)
        {

        }

        private void OnDialogEvent(IntPtr dialog, int eventCode, BVCU.BVCU_Result errorCode, int mediaDir)
        {

        }

        private void OnGetPuList(IntPtr session, IntPtr puId, IntPtr puName, int status, ref BVCU_PUOneChannelInfo channel, int finished)
        {
            var id = Marshal.PtrToStringAnsi(puId, BVCU.BVCU_MAX_ID_LEN + 1).Split('\0')[0];
            var name = Marshal.PtrToStringAnsi(puName, BVCU.BVCU_MAX_NAME_LEN + 1).Split('\0')[0];
            if (m_devices != null)
            {
                var dev = m_devices.FirstOrDefault(o => o.Id != null && o.Id.ToString() == id);
                if (dev == null)
                {
                    dev = new CameraDevice()
                    {
                        Channel = new List<VideoChannel>(),
                        Name = name,
                        Id = id,
                    };
                    m_devices.Add(dev);
                }
                if (channel.iMediaDir.HasFlag(BVCU_MEDIADIR.BVCU_MEDIADIR_VIDEORECV))
                {
                    (dev.Channel as List<VideoChannel>).Add(new VideoChannel() { Camera = dev, Channel = channel.iChannelIndex, Name = channel.szName, Tag = channel });
                }
            }
            if (finished != 0)
            {
                m_gettingDevice = false;
                m_eventAggregator.PublishOnBackgroundThread(new EvtCameraGetDevices() { Devices = m_devices });
            }
        }

        private void OnProcChannelInfo(IntPtr session, IntPtr puId, IntPtr puName, int status, ref BVCU_PUOneChannelInfo channel, int finished)
        {

        }

        private void OnServerEvent(IntPtr session, int eventCode, ref BVCU_Event_Common eventCommon)
        {
            var message = eventCommon.errorCode.ToString();
            switch (eventCode)
            {
                case BVCU.BVCU_EVENT_SESSION_OPEN://建立连接事件
                    switch (eventCommon.errorCode)
                    {
                        case BVCU.BVCU_Result.BVCU_RESULT_S_OK:
                        case BVCU.BVCU_Result.BVCU_RESULT_S_IGNORE:
                        case BVCU.BVCU_Result.BVCU_RESULT_S_PENDING:
                            IsConnected = true;
                            OnStatusChanged(message);
                            //连接成功。
                            break;
                        case BVCU.BVCU_Result.BVCU_RESULT_E_CONNECTFAILED:
                            OnError(CameraError.ConnectFailed, message);
                            break;
                        case BVCU.BVCU_Result.BVCU_RESULT_E_TIMEOUT:
                            OnError(CameraError.ConnectTimeout, message);
                            break;
                        default:
                            this.Warn("video service connect failed.{0}", eventCommon.errorCode);
                            break;
                    }
                    break;
                case BVCU.BVCU_EVENT_SESSION_CLOSE://关闭连接事件
                    switch (eventCommon.errorCode)
                    {
                        case BVCU.BVCU_Result.BVCU_RESULT_S_OK:
                        case BVCU.BVCU_Result.BVCU_RESULT_S_IGNORE:
                        case BVCU.BVCU_Result.BVCU_RESULT_S_PENDING:
                            IsConnected = false;
                            OnStatusChanged(message);
                            //正常断开。
                            break;
                        case BVCU.BVCU_Result.BVCU_RESULT_E_DISCONNECTED://被动断开
                            IsConnected = false;
                            OnStatusChanged();
                            OnError(CameraError.Disconnected, message);
                            break;
                        default:
                            this.Warn("video service disconnect unknow error.{0}", eventCommon.errorCode);
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnStatusChanged(string message = null)
        {
            m_eventAggregator.PublishOnBackgroundThread(new EvtCameraConnect() { IsConnected = IsConnected, Message = message });
        }

        private void OnError(CameraError error, string message = null)
        {
            this.Warn(" error {0},{1}", error, message);
            m_eventAggregator.PublishOnBackgroundThread(new EvtCameraError() { Error = error, ErrorMessage = message });

        }

        private void Disconnect()
        {
            if (m_dialogHandle != IntPtr.Zero)
            {
                BVCU.ManagedLayer_CuCloseDialog(m_sdkHandle, m_dialogHandle);
                m_dialogHandle = IntPtr.Zero;
            }
            if (m_sessionHandler != IntPtr.Zero)
            {
                BVCU.ManagedLayer_CuLogout(m_sdkHandle, m_sessionHandler);
                m_sessionHandler = IntPtr.Zero;
            }
        }

        private void Connect()
        {
            var ip = Setting.Host;
            var port = Setting.Port;
            var userName = Setting.UserName;
            var password = Setting.Password;
            var timeout = Setting.ConnectionTimeout;
            var ret = BVCU.ManagedLayer_CuLogin(m_sdkHandle,
                     ref m_sessionHandler, Encoding.UTF8.GetBytes(ip),
                     port, Encoding.UTF8.GetBytes(userName),
                     Encoding.UTF8.GetBytes(password), timeout,
                     m_serverEvent, m_serverProcChannelInfo);
            BVCU.FAILED(ret);
        }

        public void Dispose()
        {
            Disconnect();
            if (m_sdkHandle != IntPtr.Zero)
            {
                BVCU.ManagedLayer_CuRelease(m_sdkHandle);
                m_sdkHandle = IntPtr.Zero;
            }
        }

        public void OpenVideo(object winPanel, VideoChannel channel = null)
        {
            if (winPanel is Control control)
            {
                if (channel == null && m_devices != null && m_devices.Any())
                {
                    var camera = m_devices.FirstOrDefault(o => o.Id == Setting.CameraId);
                    if (camera != null)
                    {
                        channel = camera.Channel.FirstOrDefault();
                    }
                }
                var chnl = Setting.ChannelIndex;
                var puId = Setting.CameraId;
                if (channel != null)
                {
                    chnl = channel.Channel;
                    puId = channel.Camera.Id;
                }
                var width = control.Width;
                var height = control.Height;
                var dispRect = new BVRect(0, 0, width, height);
                var net = new BVCU_DialogControlParam_Network(0, 5, 1, 3);
                var ret = BVCU.ManagedLayer_CuBrowsePu(m_sdkHandle,//sdk handle
                                  ref m_dialogHandle,//dialog handle
                                  m_sessionHandler,//session handle
                                  Encoding.UTF8.GetBytes(puId),//pu id
                                  chnl, //channel no                            
                                  control.Handle, //pannel handle
                                  ref dispRect,//上下左右，矩形
                                  Setting.Volumes,//音量
                                  0,//single Rec File Sec
                                  Encoding.UTF8.GetBytes(""), //rec File Dir
                                  true,//videoTrans
                                  ref net,//network 时延
                                  m_dialog_OnDialogEvent,
                                  m_dialog_OnStorageEvent);
                BVCU.FAILED(ret);
            }
            else
            {
                throw new ArgumentException("winPanel 必须是 win32 Control对象。");
            }
        }

        public void GetDevices()
        {
            lock (this)
            {
                if (m_gettingDevice)
                    return;
                m_gettingDevice = true;
            }
            m_devices = new List<CameraDevice>();
            var ret = BVCU.ManagedLayer_CuGetPuList(m_sdkHandle, m_sessionHandler, m_getPuList);
            BVCU.FAILED(ret);
        }

        public async void Reconnect()
        {
            Disconnect();
            Connect();
            await Task.Delay(1000);
            GetDevices();
        }
    }
}

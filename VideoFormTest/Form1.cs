//using AirMonitor.Camera;
using BVCUSDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoFormTest
{
    public partial class Form1 : Form
    {
        private IntPtr m_sdkHandle;
        private IntPtr m_sessionHandler;
        private BVCU_Cmd_OnGetPuList m_getPuList;
        private BVCU_Server_OnEvent m_serverEvent;
        private BVCU_Server_ProcChannelInfo m_serverProcChannelInfo;
        private BVCU_Dialog_OnDialogEvent m_dialog_OnDialogEvent;
        private BVCU_Dialog_OnStorageEvent m_dialog_OnStorageEvent;
        private IntPtr m_dialogHandle;
        private BVCU.DisplayFont m_afterRenderDisplayFont;

        //private List<CameraDevice> m_devices;
        public CameraSetting Setting { get; private set; }

        public Form1()
        {
            InitializeComponent();
            m_serverEvent = new BVCU_Server_OnEvent(OnServerEvent);
            m_serverProcChannelInfo = new BVCU_Server_ProcChannelInfo(OnProcChannelInfo);
            m_getPuList = new BVCU_Cmd_OnGetPuList(OnGetPuList);
            m_dialog_OnDialogEvent = new BVCU_Dialog_OnDialogEvent(OnDialogEvent);
            m_dialog_OnStorageEvent = new BVCU_Dialog_OnStorageEvent(OnStorageEvent);
            m_afterRenderDisplayFont = new BVCU.DisplayFont(OnDisplatFont);
            Onload();
        }

        private void OnDisplatFont(IntPtr dialog, long timeStamp)
        {
        }

        private void OnStorageEvent(IntPtr dialog, int eventCode, int errorCode, IntPtr fileName, int strLen, long timeStamp)
        {

        }

        private void OnDialogEvent(IntPtr dialog, int eventCode, int errorCode, int mediaDir)
        {

        }

        private void OnGetPuList(IntPtr session, IntPtr puId, IntPtr puName, int status, ref BVCU_PUOneChannelInfo channel, int finished)
        {
            //var id = Marshal.PtrToStringAnsi(puId, BVCU.BVCU_MAX_ID_LEN + 1).Split('\0')[0];
            //var name = Marshal.PtrToStringAnsi(puName, BVCU.BVCU_MAX_NAME_LEN + 1).Split('\0')[0];
            //if (m_devices != null)
            //{
            //    var dev = m_devices.FirstOrDefault(o => o.Id != null && o.Id.ToString() == id);
            //    if (dev == null)
            //    {
            //        dev = new CameraDevice()
            //        {
            //            Channel = new List<VideoChannel>(),
            //            Name = name,
            //            Id = id,
            //        };
            //        m_devices.Add(dev);
            //    }
            //    if (channel.iMediaDir.HasFlag(BVCU_MEDIADIR.BVCU_MEDIADIR_VIDEORECV))
            //    {
            //        (dev.Channel as List<VideoChannel>).Add(new VideoChannel()
            //        {
            //            Camera = dev,
            //            Channel = channel.iChannelIndex,
            //            Name = channel.szName,
            //            IsOnline = status != BVCU.BVCU_ONLINE_STATUS_OFFLINE,
            //            Tag = channel
            //        });
            //    }
            //}
            //if (finished != 0)
            //{
            //    m_gettingDevice = false;
            //}
        }

        private void OnProcChannelInfo(IntPtr session, IntPtr puId, IntPtr puName, int status, ref BVCU_PUOneChannelInfo channel, int finished)
        {

        }

        private void OnServerEvent(IntPtr session, int eventCode, ref BVCU_Event_Common eventCommon)
        {
        }



        public void OpenVideo(object winPanel)
        {
            if (winPanel is System.Windows.Forms.Control control)
            {
                var width = control.Width;
                var height = control.Height;
                var dispRect = new BVRect(0, 0, 400, 300);
                var net = new BVCU_DialogControlParam_Network(0, 5, 1, 3);
                var ret = BVCU.ManagedLayer_CuBrowsePu(m_sdkHandle,//sdk handle
                                  ref m_dialogHandle,//dialog handle
                                  m_sessionHandler,//session handle
                                  Encoding.UTF8.GetBytes("PU_7116"),//pu id
                                  0, //channel no                            
                                  control.Handle, //pannel handle
                                  ref dispRect,//上下左右，矩形
                                  0,//音量
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

        private async void Onload()
        {

            BVCU.FAILED(BVCU.ManagedLayer_CuInit(ref m_sdkHandle));
            Setting = CreateCameraSetting();
            await Task.Delay(1000);
            Reconnect();
            await Task.Delay(1000);
            OpenVideo(panel1);
        }


        private CameraSetting CreateCameraSetting()
        {
            return new CameraSetting()
            {
                Host = "47.92.130.204",
                Port = 9701,
                UserName = "20180208",
                Password = "123456",
                ConnectionTimeout = 30,
                Volumes = 80,
                CameraId = null,
                VideoChanel = 0,
            };
        }

        public class CameraSetting
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public int ConnectionTimeout { get; set; }
            public int Volumes { get; set; }
            public int VideoChanel { get; set; }
            public string CameraId { get; set; }
        }

        public async void Reconnect()
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
            await Task.Delay(1000);
            BVCU.ManagedLayer_DispSetDisplayFontFunc(m_afterRenderDisplayFont);

        }
    }
}

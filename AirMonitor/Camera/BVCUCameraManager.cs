using AirMonitor.Interfaces;
using BVCUSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static BVCUSDK.BVCU.EventHandler;

namespace AirMonitor.Camera
{
    class BVCUCameraManager : ICameraManager
    {
        private readonly IntPtr m_sdkHandle;
        private readonly BVCU_Cmd_OnGetPuList m_getPuList;
        private IntPtr m_sessionHandler;
        private BVCU_Server_OnEvent m_serverEvent;
        private BVCU_Server_ProcChannelInfo m_serverProcChannelInfo;
        private List<BVCU_PUOneChannelInfo> m_channelList;
        private IntPtr m_dialogHandle;
        private List<Tuple<string, string>> m_channel;
        private BVCU_Dialog_OnDialogEvent m_dialog_OnDialogEvent;
        private BVCU_Dialog_OnStorageEvent m_dialog_OnStorageEvent;

        public BVCUCameraManager()
        {
            BVCU.FAILED(BVCU.ManagedLayer_CuInit(ref m_sdkHandle));
            var ip = "47.92.130.204";
            var port = 9701;
            var userName = "20180208";
            var password = "123456";
            var timeout = 30;
            m_serverEvent = new BVCU_Server_OnEvent(OnServerEvent);
            m_serverProcChannelInfo = new BVCU_Server_ProcChannelInfo(OnProcChannelInfo);
            m_getPuList = new BVCU_Cmd_OnGetPuList(OnGetPuList);
            m_dialog_OnDialogEvent = new BVCU_Dialog_OnDialogEvent(OnDialogEvent);
            m_dialog_OnStorageEvent = new BVCU_Dialog_OnStorageEvent(OnStorageEvent);
            var ret = BVCU.ManagedLayer_CuLogin(m_sdkHandle,
                     ref m_sessionHandler, Encoding.UTF8.GetBytes(ip),
                     port, Encoding.UTF8.GetBytes(userName),
                     Encoding.UTF8.GetBytes(password), timeout,
                     m_serverEvent, m_serverProcChannelInfo);
            BVCU.FAILED(ret);
            m_channelList = new List<BVCU_PUOneChannelInfo>();
            m_channel = new List<Tuple<string, string>>();
            OnGetPuList();
        }

        private void OnStorageEvent(IntPtr dialog, int eventCode, int errorCode, IntPtr fileName, int strLen, long timeStamp)
        {
            throw new NotImplementedException();
        }

        private void OnDialogEvent(IntPtr dialog, int eventCode, int errorCode, int mediaDir)
        {
        }

        private async void OnGetPuList()
        {
            await Task.Delay(1000);
            var ret = BVCU.ManagedLayer_CuGetPuList(m_sdkHandle, m_sessionHandler, m_getPuList);
            BVCU.FAILED(ret);
        }

        private void OnGetPuList(IntPtr session, IntPtr puId, IntPtr puName, int status, ref BVCU_PUOneChannelInfo channel, int finished)
        {
            string puId1 = Marshal.PtrToStringAnsi(puId, BVCU.BVCU_MAX_ID_LEN + 1).Split('\0')[0];
            string puName1 = Marshal.PtrToStringAnsi(puName, BVCU.BVCU_MAX_NAME_LEN + 1).Split('\0')[0];
            m_channel.Add(Tuple.Create(puId1, puName1));
            m_channelList.Add(channel);
        }

        private void OnProcChannelInfo(IntPtr session, IntPtr puId, IntPtr puName, int status, ref BVCU_PUOneChannelInfo channel, int finished)
        {

        }

        private void OnServerEvent(IntPtr session, int eventCode, ref BVCU_Event_Common eventCommon)
        {
            //m_sessionHandler = session;
        }

        public void Dispose()
        {
        }

        public void Open(IntPtr pWnd)
        {
            var chnl = m_channelList.FirstOrDefault();
            var chnl1 = m_channel.FirstOrDefault();
            var dispRect = new BVRect(0, 0, 300, 400);
            BVCU_DialogControlParam_Network net = new BVCU_DialogControlParam_Network(0, 5, 1, 3);

            var channelNo = 0;
            var volume = 0;
            var ret = BVCU.ManagedLayer_CuBrowsePu(m_sdkHandle,//sdk handle
                              ref m_dialogHandle,//dialog handle
                              m_sessionHandler,//session handle
                              Encoding.UTF8.GetBytes(chnl1.Item1),//pu id
                              channelNo, //channel no                            
                              pWnd, //pannel handle
                              ref dispRect,//上下左右，矩形
                              volume,//音量
                              0,//single Rec File Sec
                              Encoding.UTF8.GetBytes(""), //rec File Dir
                              true,//videoTrans
                              ref net,//network 时延
                              m_dialog_OnDialogEvent,
                              m_dialog_OnStorageEvent);
            BVCU.FAILED(ret);
        }
    }
}

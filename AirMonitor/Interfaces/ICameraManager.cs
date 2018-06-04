using AirMonitor.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Interfaces
{
    /// <summary>
    /// 摄像头管理器。
    /// </summary>
    public interface ICameraManager : IDisposable
    {
        /// <summary>
        /// 在指定窗口打开视频通道。
        /// </summary>
        /// <param name="winPanel">Win32窗体。</param>
        /// <param name="channel">视频通道。可以通过<see cref="GetDevices"/>获得，视频通道为空时，默认会读取<see cref="CameraSetting"/></param>
        void OpenVideo(object winPanel, VideoChannel channel = null);
        /// <summary>
        /// 获取设备列表。以事件方式返回。
        /// </summary>
        void GetDevices();
        /// <summary>
        /// 刷新摄像头连接。
        /// </summary>
        void Reconnect();

    }
}

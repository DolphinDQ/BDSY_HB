using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Camera
{
    public enum CameraError
    {
        /// <summary>
        /// 连接错误导致断开。
        /// </summary>
        Disconnected,
        /// <summary>
        /// 连接超时。
        /// </summary>
        ConnectTimeout,
        /// <summary>
        /// 连接服务器失败。
        /// </summary>
        ConnectFailed

    }
}

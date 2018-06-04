using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Camera
{
    /// <summary>
    /// 摄像头管理器状态。
    /// </summary>
    public enum CameraManagerStatus
    {
        Disconnected,
        Connected,
        IncorrectUser,
    }
}

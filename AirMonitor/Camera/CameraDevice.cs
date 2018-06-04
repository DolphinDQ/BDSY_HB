using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Camera
{
    /// <summary>
    /// 摄像头设备。摄像头可能有几个视频通道。
    /// </summary>
    public class CameraDevice
    {
        /// <summary>
        /// 设备名称。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 预留。
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 摄像头所包含通道。
        /// </summary>
        public IEnumerable<VideoChannel> Channel { get; set; }
    }
}

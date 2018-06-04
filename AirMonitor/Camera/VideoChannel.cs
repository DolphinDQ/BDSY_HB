using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Camera
{
    /// <summary>
    /// 视频通道。
    /// </summary>
    public class VideoChannel
    {
        /// <summary>
        /// 摄像头
        /// </summary>
        public CameraDevice Camera { get; set; }
        /// <summary>
        /// 通道名称。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 通道号码。
        /// </summary>
        public int Channel { get; set; }
        /// <summary>
        /// 在线。
        /// </summary>
        public bool IsOnline { get; set; }
        /// <summary>
        /// 预留。
        /// </summary>
        public object Tag { get; set; }
    }
}

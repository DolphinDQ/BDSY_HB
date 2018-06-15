using AirMonitor.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Config
{
    /// <summary>
    /// 空气采样存档。
    /// </summary>
    public class AirSamplesSave
    {
        /// <summary>
        /// 说明信息。
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 开始时间。
        /// </summary>
        public DateTime? Since { get; set; }
        /// <summary>
        /// 持续时间。
        /// </summary>
        public DateTime? Until { get; set; }
        /// <summary>
        /// 覆盖面积。平方米
        /// </summary>
        public int Acreage { get; set; }
        /// <summary>
        /// 采样标准设置。
        /// </summary>
        public AirStandardSetting Standard { get; set; }
        /// <summary>
        /// 采样集合。
        /// </summary>
        public EvtAirSample[] Samples { get; set; }
        /// <summary>
        /// 持续时间
        /// </summary>
        public TimeSpan? Duration { get; set; }
    }
}

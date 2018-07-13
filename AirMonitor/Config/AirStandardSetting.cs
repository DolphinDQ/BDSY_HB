using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Config
{
    public class AirStandardSetting
    {
        /// <summary>
        /// 污染物标准。
        /// </summary>
        public AirPollutant[] Pollutant { get; set; }
        /// <summary>
        /// 相对海拔。
        /// </summary>
        public double CorrectAltitude { get; set; }
        /// <summary>
        /// 最高海拔限制。
        /// </summary>
        public double MaxAltitude { get; set; }
        /// <summary>
        /// 高度单位。
        /// </summary>
        public string AltitudeUnit { get; set; }
        /// <summary>
        /// 标识透明度。
        /// </summary>
        public double Opacity { get; set; }
        /// <summary>
        /// 边长。地图上显示探测范围（方块边长）。
        /// </summary>
        public double SideLength { get; set; }
        /// <summary>
        /// 获取标准服务器
        /// </summary>
        public string Server { get; set; }

    }
}

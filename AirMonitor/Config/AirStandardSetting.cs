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
    }
}

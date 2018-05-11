using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Config
{
    /// <summary>
    /// 空气污染物。
    /// </summary>
    public class AirPollutant
    {
        /// <summary>
        /// 污染物名称。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 最大值。
        /// </summary>
        public double MaxValue { get; set; }
        /// <summary>
        /// 最小值。
        /// </summary>
        public double MinValue { get; set; }
        /// <summary>
        /// 显示名称。
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 单位。
        /// </summary>
        public string Unit { get; set; }
    }
}

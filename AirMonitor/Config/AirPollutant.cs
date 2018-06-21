using Newtonsoft.Json;
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
        public double MaxValue => Levels?.Max(o => o.MaxValue) ?? 0;
        /// <summary>
        /// 最小值。
        /// </summary>
        public double MinValue => Levels?.Min(o => o.MinValue) ?? 0;
        /// <summary>
        /// 显示名称。
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 单位。
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 等级。
        /// </summary>
        public AirPollutantLevel[] Levels { get; set; }

    }
}

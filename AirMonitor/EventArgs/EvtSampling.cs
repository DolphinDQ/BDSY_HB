using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.EventArgs
{
    /// <summary>
    /// 采样事件。
    /// </summary>
    public class EvtSampling
    {
        /// <summary>
        /// 采样状态。
        /// </summary>
        public SamplingStatus Status { get; set; }
    }

    public enum SamplingStatus
    {
        Stop,
        Start,
        ClearAll,
        StartSim,
        StopSim,
    }
}

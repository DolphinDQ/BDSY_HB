using AirMonitor.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Map
{
    /// <summary>
    /// 采用方块的报表。
    /// </summary>
    public class MapBlockReport
    {
        public AirPollutant pollutant { get; set; }
        public double count { get; set; }
        public double avg { get; set; }
        public double sum { get; set; }
        public double max { get; set; }
        public double min { get; set; }
    }
}

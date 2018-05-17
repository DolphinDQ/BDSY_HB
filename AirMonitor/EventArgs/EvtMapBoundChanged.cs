using AirMonitor.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.EventArgs
{
    public class EvtMapBoundChanged
    {
        /// <summary>
        /// 西南方向。
        /// </summary>
        public MapPoint sw { get; set; }
        /// <summary>
        /// 东北方向。
        /// </summary>
        public MapPoint en { get; set; }
    }
}

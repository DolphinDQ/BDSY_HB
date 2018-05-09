using AirMonitor.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.EventArgs
{
    /// <summary>
    /// 地图点转换后回调
    /// </summary>
    public class EvtMapPointConverted
    {
        public int Seq { get; set; }

        public MapPoint[] Points { get; set; }
    }
}

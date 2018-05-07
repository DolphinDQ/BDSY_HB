using AirMonitor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.EventArgs
{
    /// <summary>
    /// 地图加载事件。
    /// </summary>
    public class EvtMapLoad
    {

        public IMapProvider Provider { get; set; }

        public string Url { get; set; }
    }
}

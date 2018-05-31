using AirMonitor.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.EventArgs
{
    public class EvtMapUavChanged
    {
        public MapUav[] uav { get; set; }
    }
}

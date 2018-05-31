using AirMonitor.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.EventArgs
{
    public class EvtMapBlockChanged
    {
        public MapBlock[] blocks { get; set; }
    }
}

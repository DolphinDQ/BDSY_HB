using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Map
{
    public class MapBlock
    {
        public MapPoint center { get; set; }

        public MapPointData[] points { get; set; }
    }
}

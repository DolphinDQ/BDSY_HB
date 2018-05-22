using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Chart
{
    public class ScatterData
    {
   
        public double X { get; set; }

        public double Y { get; set; }

        public double Value { get; set; } = double.NaN;

        public object Tag { get; set; }

        public double Size { get; set; } = double.NaN;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Chart
{
    public class ScatterOptions : ChartOptions
    {
        public double? MaxVaule { get; set; }
        public double? MinValue { get; set; }

        public string MaxColor { get; set; }

        public string MinColor { get; set; }
    }
}

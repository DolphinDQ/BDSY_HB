using AirMonitor.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.EventArgs
{
    /// <summary>
    /// 
    /// </summary>
    public class EvtChartScatterSelectChanged
    {
        public object Scatter { get; set; }

        public IEnumerable<ScatterData> Data { get; set; }
    }
}

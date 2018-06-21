using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Config
{
    /// <summary>
    /// 空气质量等级。
    /// </summary>
    public class AirPollutantLevel
    {
        public string Name { get; set; }

        public double MinValue { get; set; }

        public double MaxValue { get; set; }

        public string MaxColor { get; set; }

        public string MinColor { get; set; }
    }
}

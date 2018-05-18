using AirMonitor.Interfaces;
using AirMonitor.Map;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.ViewModels
{
    public class SampleAnalysisViewModel : Screen
    {
        public enum AnalysisMode
        {
            Vertical,
            Horizontal,
        }

        public IMapView MapView { get; set; }

        public MapBlock[] MapBlocks { get; set; }

        public AnalysisMode Mode { get; set; }

    }
}

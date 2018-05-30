using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Map
{
    public enum MapEvents
    {
        load,
        pointConvert,
        horizontalAspect,
        verticalAspect,
        clearAspect,
        selectAnalysisArea,
        clearAnalysisArea,
        savePoints,
        boundChanged,
        blockChanged,
        uavChanged,
    }
}

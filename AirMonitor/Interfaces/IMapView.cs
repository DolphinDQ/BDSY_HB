using System;
using System.Collections.Generic;
using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using AirMonitor.Map;

namespace AirMonitor.Interfaces
{
    public interface IMapView
    {
        Tuple<string, string> DataName { get; set; }
        List<Tuple<string, string>> DataNameList { get; set; }
        bool MapLoad { get; }
        IMapProvider MapProvider { get; }
        List<EvtAirSample> Samples { get; }
        bool Sampling { get; }
        MapGridOptions MapGridOptions { get; }
    }
}
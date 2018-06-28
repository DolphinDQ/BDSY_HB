using AirMonitor.Chart;
using AirMonitor.EventArgs;
using System;
using System.Collections.ObjectModel;

namespace AirMonitor.Interfaces
{
    /// <summary>
    /// 报表管理器。
    /// </summary>
    public interface IChartManager
    {
        object CreateLiner(ObservableCollection<Tuple<DateTime, double>> data, LinerOptions options = null);

        object CreateLiner(ObservableCollection<EvtAirSample> data, string xKey, string yKey);

        object CreateScatter(ObservableCollection<ScatterData> data, ScatterOptions options = null);

        void SetScatter(object scatter, ScatterOptions options);
    }

}

using AirMonitor.Chart;
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

        object CreateScatter(ObservableCollection<ScatterData> data, ScatterOptions options = null);

        void SetScatter(object scatter, ScatterOptions options);
    }

}

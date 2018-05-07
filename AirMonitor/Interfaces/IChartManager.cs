using System;
using System.Collections.ObjectModel;

namespace AirMonitor.Interfaces
{
    /// <summary>
    /// 报表管理器。
    /// </summary>
    public interface IChartManager
    {
        /// <summary>
        /// 创建线性报表。
        /// </summary>
        /// <returns></returns>
        object CreatLiner(ObservableCollection<Tuple<DateTime, double>> data);

    }

}

using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Interfaces
{
    public interface IChartProvider
    {
        /// <summary>
        /// 创建线性报表。
        /// </summary>
        /// <returns></returns>
        object CreatLiner(IObservableCollection<Tuple<DateTime, double>> data);

    }

}

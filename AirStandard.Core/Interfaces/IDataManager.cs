using AirStandard.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirStandard.Core.Interfaces
{
    public interface IDataManager : IDisposable
    {
        void Init();
        /// <summary>
        /// 获取指定日期的采样。
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        Task<IEnumerable<StandardSample>> GetSample(DateTime date);
    }
}

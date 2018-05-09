using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Interfaces
{
    /// <summary>
    /// 存档管理器。
    /// </summary>
    public interface ISaveManager
    {
        /// <summary>
        /// 数据保存。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        void Save(string path, object data);
        /// <summary>
        /// 数据加载。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        T Load<T>(string path);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Interfaces
{
    /// <summary>
    /// 对象工厂。用于创建对象。
    /// </summary>
    public interface IFactory
    {
        T Create<T>(string name = null);
    }
}

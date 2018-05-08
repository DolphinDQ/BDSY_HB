using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Interfaces
{
    /// <summary>
    /// 地图资源提供者。
    /// </summary>
    public interface IMapProvider
    {
        /// <summary>
        /// 加载地图
        /// </summary>
        /// <param name="mapContainer">加载地图的容器，一般为WebBrowser</param>
        void LoadMap(object mapContainer);
        /// <summary>
        /// 调用JS函数。
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="methodName">函数名称。</param>
        /// <param name="args">参数</param>
        /// <returns>返回值。</returns>
        T Invoke<T>(string methodName, params object[] args);

        T Invoke<T>(string methodName, Func<object, T> parse, params object[] args);
        /// <summary>
        /// 调用JS函数。
        /// </summary>
        /// <param name="methodName">函数名称</param>
        /// <param name="args">参数</param>
        void Invoke(string methodName, params object[] args);
    }
}

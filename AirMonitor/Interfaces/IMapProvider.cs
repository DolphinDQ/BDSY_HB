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

    }
}

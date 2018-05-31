using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Map
{
    /// <summary>
    /// 无人机。
    /// </summary>
    public class MapUav
    {
        /// <summary>
        /// 无人机名称或标识。
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 纬度。gps坐标系。
        /// </summary>
        public double lat { get; set; }
        /// <summary>
        /// 经度。gps坐标系。
        /// </summary>
        public double lng { get; set; }
        /// <summary>
        /// 无人机携带的数据。
        /// </summary>
        public object data { get; set; }
    }
}

using AirMonitor.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Map
{
    public static class MapEx
    {
        /// <summary>
        /// 初始化网格。修改参数可以重新初始化。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="options"></param>
        public static void GridInit(this IMapProvider map, MapGridOptions options)
        {
            map.Invoke("gridInit", JsonConvert.SerializeObject(options));
        }
        /// <summary>
        /// 清除当前已经绘制的网格。
        /// </summary>
        /// <param name="map"></param>
        public static void GridClear(this IMapProvider map)
        {
            map.Invoke("gridClear");
        }
        /// <summary>
        /// 刷新或重新加载网格。
        /// </summary>
        /// <param name="map"></param>
        public static void GridRefresh(this IMapProvider map)
        {
            map.Invoke("gridRefresh");
        }
        /// <summary>
        /// 添加无人机。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="uav"></param>
        public static void UavAdd(this IMapProvider map, Uav uav)
        {
            map.Invoke("uavAdd", uav.name, uav.lng, uav.lat, JsonConvert.SerializeObject(uav.data));
        }
        /// <summary>
        /// 移动无人机。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="uav"></param>
        public static void UavMove(this IMapProvider map, Uav uav)
        {
            map.Invoke("uavMove", uav.name, uav.lng, uav.lat, JsonConvert.SerializeObject(uav.data));
        }
        /// <summary>
        /// 显示/刷新路径，隐藏路径。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="show">true为显示或者刷新，false为隐藏。</param>
        public static void UavPath(this IMapProvider map, string name, bool show)
        {
            if (show)
            {
                map.Invoke("uavShowPath", name);
            }
            else
            {
                map.Invoke("uavHidePath", name);
            }
        }
        /// <summary>
        /// 移除无人机。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="name"></param>
        public static void UavRemove(this IMapProvider map, string name)
        {
            map.Invoke("uavRemove", name);
        }
        /// <summary>
        /// 是否存在无人机。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="name"></param>
        public static bool UavExist(this IMapProvider map, string name)
        {
            return map.Invoke("uavExist", o => bool.Parse(o.ToString()), name);
        }
        /// <summary>
        /// 地图坐标转换。转换结果通过EvtMapPointConverted事件回调。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="seq"></param>
        /// <param name="points"></param>
        public static void MapPointConvert(this IMapProvider map, int seq, MapPoint[] points)
        {
            map.Invoke("mapPointConvert", seq, JsonConvert.SerializeObject(points));
        }
        /// <summary>
        /// 无人机跟踪。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="name"></param>
        public static void UavFocus(this IMapProvider map, string name)
        {
            map.Invoke("uavFocus", name);
        }
    }
}

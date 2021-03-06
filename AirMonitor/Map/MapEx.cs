﻿using AirMonitor.Interfaces;
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
        #region 地图
        /// <summary>
        /// 地图坐标转换。转换结果通过EvtMapPointConverted事件回调。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="seq"></param>
        /// <param name="points"></param>
        public static void MapPointConvert(this IMapProvider map, int seq, MapPoint[] points)
        {
            map.Invoke("map.mapPointConvert", seq, JsonConvert.SerializeObject(points));
        }
        /// <summary>
        /// 显示地图报表。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="points"></param>
        public static void MapShowTempReport(this IMapProvider map, MapPointData[] points)
        {
            map.Invoke("map.mapShowTempReport", JsonConvert.SerializeObject(points));
        }

        public static void MapClearTempReport(this IMapProvider map)
        {
            map.Invoke("map.mapClearTempReport");
        }
        /// <summary>
        /// 初始化地图右键菜单。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="edit">编辑模式。</param>
        public static void MapInitMenu(this IMapProvider map, bool edit)
        {
            map.Invoke("map.mapInitMenu", edit);
        }
        /// <summary>
        /// 设置地图样式。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="style"></param>
        public static void MapStyle(this IMapProvider map, string style)
        {
            map.Invoke("map.mapStyle", style);
        }

        ///// <summary>
        ///// 地图边界变更事件。
        ///// </summary>
        ///// <param name="map"></param>
        ///// <param name="subscribe"></param>
        //public static void MapBoundChangedEvent(this IMapProvider map, bool subscribe)
        //{
        //    map.Invoke("map.mapPointConvert", subscribe);
        //}
        #endregion
        #region 网格

        /// <summary>
        /// 初始化网格。修改参数可以重新初始化。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="options"></param>
        public static void GridInit(this IMapProvider map, MapGridOptions options)
        {
            map.Invoke("map.gridInit", JsonConvert.SerializeObject(options));
        }
        /// <summary>
        /// 清除当前已经绘制的网格。
        /// </summary>
        /// <param name="map"></param>
        public static void GridClear(this IMapProvider map)
        {
            map.Invoke("map.gridClear");
        }
        /// <summary>
        /// 刷新或重新加载网格。
        /// </summary>
        /// <param name="map"></param>
        public static void GridRefresh(this IMapProvider map)
        {
            map.Invoke("map.gridRefresh");
        }

        #endregion
        #region 无人机


        /// <summary>
        /// 添加无人机。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="uav"></param>
        public static void UavAdd(this IMapProvider map, MapUav uav)
        {
            map.Invoke("map.uavAdd", uav.name, uav.lng, uav.lat, JsonConvert.SerializeObject(uav.data));
        }
        /// <summary>
        /// 移动无人机。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="uav"></param>
        public static void UavMove(this IMapProvider map, MapUav uav)
        {
            map.Invoke("map.uavMove", uav.name, uav.lng, uav.lat, JsonConvert.SerializeObject(uav.data));
        }
        /// <summary>
        /// 显示/刷新路径，隐藏路径。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="show">true为显示或者刷新，false为隐藏。</param>
        public static void UavPath(this IMapProvider map, string name)
        {
            map.Invoke("map.uavPathRefresh", name);
        }
        /// <summary>
        /// 移除无人机。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="name"></param>
        public static void UavRemove(this IMapProvider map, string name)
        {
            map.Invoke("map.uavRemove", name);
        }
        /// <summary>
        /// 是否存在无人机。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="name"></param>
        public static bool UavExist(this IMapProvider map, string name)
        {
            return map.Invoke("map.uavExist", o => bool.Parse(o.ToString()), name);
        }
        /// <summary>
        /// 无人机跟踪。
        /// </summary>
        /// <param name="map"></param>
        /// <param name="name"></param>
        public static void UavFocus(this IMapProvider map, string name)
        {
            map.Invoke("map.uavFocus", name);
        }

        public static T Subscribe<T>(this IMapProvider map, MapEvents events, bool enable)
        {
            return map.Invoke<T>("map.subscribe", events.ToString(), enable);
        }
        #endregion



    }
}

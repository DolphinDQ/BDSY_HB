﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.EventArgs
{
    /// <summary>
    /// 空气采样信息。
    /// {"co": 0.782, "pm10": 56, "temp": 28.96484, "voc": 7.582, "lon": 113.077235, "hight": 130.079, "ghight": 13.9,
    /// "humi": 70.95947, "pm25": 47, "time": "2018-05-03/13:11:38", "lat": 23.016665, "so2": 23.655, "o3": 83.944, "no2": 22.784}
    /// </summary>
    public class EvtAirSample
    {
        public string UavName { get; set; } = "default";
        public double co { get; set; }
        public double pm10 { get; set; }
        public double temp { get; set; }
        public double voc { get; set; }
        public double lon { get; set; }
        public double hight { get; set; }
        public double ghight { get; set; }
        public double humi { get; set; }
        public double pm25 { get; set; }
        public string time { get; set; }
        public double lat { get; set; }
        public double so2 { get; set; }
        public double o3 { get; set; }
        public double no2 { get; set; }
        [JsonIgnore]
        public DateTime RecordTime => DateTime.Parse(time.Replace('/', 'T'));
        [JsonIgnore]
        public double GpsLat => GpsConvert(lat);
        [JsonIgnore]
        public double GpsLng => GpsConvert(lon);
        private double GpsConvert(double source)
        {
            var i = source.ToString().Split('.');
            if (i.Length == 2)
            {
                return int.Parse(i[0]) + double.Parse(i[1].Insert(2, ".")) / 60;
            }
            return 0;
        }
        public double ActualLat { get; set; }
        public double ActualLng { get; set; }
        /// <summary>
        /// 相对海拔。
        /// </summary>
        public double RelativeHeight { get; set; }
    }
}

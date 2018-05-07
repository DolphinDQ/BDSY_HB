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
        public DateTime RecordTime => DateTime.Parse(time.Replace('/', 'T'));
    }
}

﻿using AirMonitor.Config;
using AirMonitor.EventArgs;
using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AirMonitor.Map
{
    /// <summary>
    /// 地图网格选项。
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class MapGridOptions
    {
        public MapGridOptions()
        {
            //Reload();
        }

        /// <summary>
        /// 网格边长。单位（米）
        /// </summary>
        public double sideLength { get; set; } = 15;
        ///// <summary>
        ///// 渐变色开始。格式："#ffffff"，"#0f0f0f"
        ///// </summary>
        //public string colorBegin { get; set; } = "#00ff00";
        ///// <summary>
        ///// 渐变色结束。格式："#ffffff"，"#0f0f0f"
        ///// </summary>
        //public string colorEnd { get; set; } = "#ff0000";
        /// <summary>
        /// 透明度。0-1
        /// </summary>
        public double opacity { get; set; } = 0.8;
        /// <summary>
        /// 污染物。
        /// </summary>
        public AirPollutant[] pollutants { get; set; }
        /// <summary>
        /// 当前选中的污染物。
        /// </summary>
        public AirPollutant pollutant { get; set; }

        public void OnpollutantsChanged()
        {
            pollutant = pollutants?.FirstOrDefault();
            //Reload();
        }

        //public void Reload()
        //{
        //    var maxValue = pollutant?.MaxValue ?? 0;
        //    var minValue = pollutant?.MinValue ?? 0;
        //    var val = maxValue - minValue;
        //    if (val > 0)
        //    {
        //        double[] res = new double[6];
        //        for (int i = 0; i < 5; i++)
        //        {
        //            res[i] = minValue + val / 5 * i;
        //        }
        //        res[5] = maxValue;
        //        ValueStep = res;
        //    }
        //}

        //[JsonIgnore]
        //public double[] ValueStep { get; private set; }

        public string GetColor(double value)
        {
            if (pollutant.Levels != null)
            {
                var minValue = pollutant.MinValue;
                var maxValue = pollutant.MaxValue;
                if (value > maxValue) value = maxValue;
                if (value < minValue) value = minValue;
                var level = pollutant.Levels.FirstOrDefault(o => o.MaxValue >= value && o.MinValue <= value);
                maxValue = level.MaxValue;
                minValue = level.MinValue;
                var percent = (value - minValue) / (maxValue - minValue);
                var begin = (Color)ColorConverter.ConvertFromString(level.MinColor);
                var end = (Color)ColorConverter.ConvertFromString(level.MaxColor);
                var r = GetColorValue(percent, begin.R, end.R);
                var g = GetColorValue(percent, begin.G, end.G);
                var b = GetColorValue(percent, begin.B, end.B);
                return "#" + r + g + b;
            }
            return "#ffffff";
        }

        private string GetColorValue(double percent, double begin, double end)
        {
            return ((int)((end - begin) * percent + begin)).ToString("x2");
        }
    }
}

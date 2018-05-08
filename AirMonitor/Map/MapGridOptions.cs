﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Map
{
    /// <summary>
    /// 地图网格选项。
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class MapGridOptions
    {
        public MapGridOptions()
        {
            SetValueStep();
        }

        /// <summary>
        /// 网格边长。单位（米）
        /// </summary>
        public double sideLength { get; set; } = 50;
        /// <summary>
        /// 渐变色开始。格式："ffffff"，"0f0f0f"
        /// </summary>
        public string colorBegin { get; set; } = "00ff00";
        /// <summary>
        /// 渐变色结束。格式："ffffff"，"0f0f0f"
        /// </summary>
        public string colorEnd { get; set; } = "ff0000";
        /// <summary>
        /// 透明度。0-1
        /// </summary>
        public double opacity { get; set; } = 0.5;
        /// <summary>
        /// 采集数据名称。数据字段名称
        /// </summary>
        public string dataName { get; set; }
        /// <summary>
        /// 数据最大值。
        /// </summary>
        public double maxValue { get; set; } = 100;
        /// <summary>
        /// 数据最小值。
        /// </summary>
        public double minValue { get; set; } = 0;

        public void OnmaxValueChanged() => SetValueStep();

        public void OnminValueChanged() => SetValueStep();

        private void SetValueStep()
        {
            var val = maxValue - minValue;
            if (val > 0)
            {
                double[] res = new double[6];
                for (int i = 0; i < 5; i++)
                {
                    res[i] = minValue + val / 5 * i;
                }
                res[5] = maxValue;
                ValueStep = res;
            }
        }
        [JsonIgnore]
        public double[] ValueStep { get; private set; }
    }
}
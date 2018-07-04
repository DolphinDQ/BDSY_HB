using AirMonitor.Config;
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
        ///// <summary>
        ///// 网格边长。单位（米）
        ///// </summary>
        //[JsonIgnore]
        //[DependsOn(nameof(settings))]
        //public double sideLength
        //{
        //    get => settings?.SideLength ?? 15;
        //    set
        //    {
        //        if (settings != null)
        //        {
        //            settings.SideLength = value;
        //        }
        //    }
        //}
        ///// <summary>
        ///// 透明度。0-1
        ///// </summary>
        //[JsonIgnore]
        //[DependsOn(nameof(settings))]
        //public double opacity
        //{
        //    get => settings?.Opacity ?? 0.8;
        //    set
        //    {
        //        if (settings != null)
        //        {
        //            settings.Opacity = value;
        //        }
        //    }
        //}
        ///// <summary>
        ///// 污染物。
        ///// </summary>
        //[JsonIgnore]
        //[DependsOn(nameof(settings))]
        //public AirPollutant[] pollutants { get; set; }
        /// <summary>
        /// 当前选中的污染物。
        /// </summary>
        public AirPollutant pollutant { get; set; }
        /// <summary>
        /// 配置文件。
        /// </summary>
        public AirStandardSetting settings { get; set; }


        public void OnsettingsChanged()
        {
            if (settings != null)
            {
                if (pollutant == null)
                {
                    pollutant = settings.Pollutant.FirstOrDefault();
                }
                else
                {
                    pollutant = settings.Pollutant.FirstOrDefault(o => o.Name == pollutant.Name);
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AirMonitor.Controls
{
    public abstract class MapMarker3D
    {

        public static MapMarker3D GetMapMarker(DependencyObject obj)
        {
            return (MapMarker3D)obj.GetValue(MapMarkerProperty);
        }

        public static void SetMapMarker(DependencyObject obj, MapMarker3D value)
        {
            obj.SetValue(MapMarkerProperty, value);
        }

        // Using a DependencyProperty as the backing store for MapMarker.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapMarkerProperty =
            DependencyProperty.RegisterAttached("MapMarker", typeof(MapMarker3D), typeof(MapMarker3D), new PropertyMetadata(null));



        /// <summary>
        /// 中心点位置。
        /// </summary>
        public Map3DPoint Position { get; set; }
        /// <summary>
        /// 占用区域。
        /// </summary>
        public Map3DBound Bound { get; set; }
    }
}

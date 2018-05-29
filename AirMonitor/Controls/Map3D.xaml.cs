using AirMonitor.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AirMonitor.Controls
{
    /// <summary>
    /// Map3D.xaml 的交互逻辑
    /// </summary>
    public partial class Map3D : UserControl
    {
        #region DependencyProperties Register

        // Using a DependencyProperty as the backing store for MapContainer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapContainerProperty =
            DependencyProperty.Register("MapContainer", typeof(object), typeof(Map3D),
                new PropertyMetadata(null, new PropertyChangedCallback(OnMapContainerChanged)),
                new ValidateValueCallback(OnMapContainerValidation));

        private static void OnMapContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Map3D map)
            {
                map.OnReloadMap();
            }
        }

        private static bool OnMapContainerValidation(object value)
        {
            return value == null || value is WebBrowser || value is Visual;
        }


        public double WallHeight
        {
            get { return (double)GetValue(WallHeightProperty); }
            set { SetValue(WallHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WallHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WallHeightProperty =
            DependencyProperty.Register("WallHeight", typeof(double), typeof(Map3D), new PropertyMetadata(250d, new PropertyChangedCallback(OnWallHeightChanged)));

        private static void OnWallHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Map3D map)
            {
                map.OnWallHeightChanged();
            }
        }


        public Map3DBound MapBound
        {
            get { return (Map3DBound)GetValue(BoundsProperty); }
            set { SetValue(BoundsProperty, value); }
        }

        /// <summary>
        /// 地图边界。
        /// </summary>
        public static readonly DependencyProperty BoundsProperty =
            DependencyProperty.Register("MapBound", typeof(Map3DBound), typeof(Map3D), new PropertyMetadata(null, new PropertyChangedCallback(OnMapBoundChanged)));

        private static void OnMapBoundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Map3D map)
            {
                map.OnMapBoundChanged();
            }
        }



        #endregion

        public Map3D()
        {
            InitializeComponent();
            MapBound = new Map3DBound();
            View3D.DataContext = MapBound;
        }

        public object MapContainer
        {
            get { return (object)GetValue(MapContainerProperty); }
            set { SetValue(MapContainerProperty, value); }
        }


        private void OnReloadMap()
        {
            if (MapContainer is WebBrowser b)
            {
                MapImage.ImageSource = Utils.SnapShot(b);
                var img = MapImage.ImageSource;
                if (img != null)
                {
                    Map.Positions = Point3DCollection.Parse(string.Format("-{0} 0 -{1},{0} 0 -{1},{0} 0 {1},-{0} 0 {1}", img.Width / 2, img.Height / 2));
                    var longer = img.Width > img.Height ? img.Width : img.Height;
                    Camera.Position = Point3D.Parse(string.Format("0 {0} {1}", longer, longer));
                    Camera.UpDirection = Vector3D.Parse("0,1,0");
                    Camera.LookDirection = Vector3D.Parse(string.Format("0 -{0} -{1}", longer, longer + longer / 2));
                    OnWallHeightChanged();
                }
            }
        }

        private void OnWallHeightChanged()
        {
            var img = MapImage.ImageSource;
            if (img != null)
            {
                var height = WallHeight;
                WallNorth.Positions = Point3DCollection.Parse(string.Format("-{0} {2} -{1},-{0} 0 -{1},{0} 0 -{1},{0} {2} -{1}", img.Width / 2, img.Height / 2, height));
                WallNorthPanel.Width = img.Width;
                WallNorthPanel.Height = height;

                WallSouth.Positions = Point3DCollection.Parse(string.Format("{0} {2} {1},{0} 0 {1},-{0} 0 {1},-{0} {2} {1}", img.Width / 2, img.Height / 2, height));
                WallSouthPanel.Width = img.Width;
                WallSouthPanel.Height = height;

                WallWest.Positions = Point3DCollection.Parse(string.Format("-{0} {2} {1},-{0} 0 {1},-{0} 0 -{1},-{0} {2} -{1}", img.Width / 2, img.Height / 2, height));
                WallWestPanel.Width = img.Height;
                WallWestPanel.Height = height;

                WallEast.Positions = Point3DCollection.Parse(string.Format("{0} {2} -{1},{0} 0 -{1},{0} 0 {1},{0} {2} {1}", img.Width / 2, img.Height / 2, height));
                WallEastPanel.Width = img.Height;
                WallEastPanel.Height = height;

                OnMapBoundChanged();
            }
        }

        /// <summary>
        /// 实际高度转换3D高度。
        /// </summary>
        /// <param name="height">GPS高度</param>
        /// <returns></returns>
        private double HeightConvert(double height)
        {
            var bound = MapBound;
            if (bound != null)
            {
                var max = bound.Max.Height;
                var min = bound.Min.Height;
                var diff = max - min;
                return diff == 0 ? double.NaN : WallHeight / diff * height;
            }
            return 0;
        }
        /// <summary>
        /// 纬度转换。
        /// </summary>
        /// <param name="lat"></param>
        /// <returns></returns>
        private double LatConvert(double lat)
        {
            var bound = MapBound;
            var img = MapImage.ImageSource;
            if (bound != null && img != null)
            {
                var max = bound.Max.Lat;
                var min = bound.Min.Lat;
                var diff = max - min;
                return diff == 0 ? double.NaN : (img.Height / diff * lat) - (img.Height / 2);
            }
            return 0;
        }
        /// <summary>
        /// 经度转换。
        /// </summary>
        /// <param name="lng"></param>
        /// <returns></returns>
        private double LngConvert(double lng)
        {
            var bound = MapBound;
            var img = MapImage.ImageSource;
            if (bound != null && img != null)
            {
                var max = bound.Max.Lng;
                var min = bound.Min.Lng;
                var diff = max - min;
                return diff == 0 ? double.NaN : (img.Width / diff * lng) - (img.Width / 2);
            }
            return 0;
        }

        private void OnMapBoundChanged()
        {
            var bound = MapBound;
            if (bound != null)
            {
                DrawWallGrid(WallNorthPanel, bound.Max.Lng, bound.Min.Lng, bound.Max.Height, bound.Min.Height);
                DrawWallGrid(WallSouthPanel, bound.Max.Lng, bound.Min.Lng, bound.Max.Height, bound.Min.Height, true);
                DrawWallGrid(WallEastPanel, bound.Max.Lat, bound.Min.Lat, bound.Max.Height, bound.Min.Height, true);
                DrawWallGrid(WallWestPanel, bound.Max.Lat, bound.Min.Lat, bound.Max.Height, bound.Min.Height);
            }
        }

        private void DrawWallGrid(Canvas canvas, double maxWidth, double minWidth, double maxHeight, double minHeight, bool reversed = false)
        {
            canvas.Children.Clear();
            var lines = 5;
            var color = System.Windows.Media.Brushes.LightGray;
            if (canvas.Width > 0)
            {
                var unitWidth = canvas.Width / lines;
                var unitWidthValue = (maxWidth - minWidth) / lines;
                for (int i = 0; i < lines; i++)
                {
                    canvas.Children.Add(new Line() { Stroke = color, X1 = i * unitWidth, Y1 = 0, X2 = i * unitWidth, Y2 = canvas.Height });
                    var lbl = new Label() { Foreground = color, Content = (i == 0 ? maxHeight + "m/" : "") + (unitWidthValue * (reversed ? lines - i : i) + minWidth) + "°" };
                    lbl.SetValue(Canvas.LeftProperty, i * unitWidth);
                    canvas.Children.Add(lbl);
                }
            }

            if (canvas.Height > 0)
            {
                var unitHeight = canvas.Height / lines;
                var unitHeightValue = (maxHeight - minHeight) / lines;
                for (int i = 0; i < 5; i++)
                {
                    canvas.Children.Add(new Line() { Stroke = color, X1 = 0, Y1 = i * unitHeight, X2 = canvas.Width, Y2 = i * unitHeight });
                    if (i > 0)
                    {
                        var lbl = new Label() { Foreground = color, Content = unitHeightValue * (lines - i) + minHeight + "m" };
                        lbl.SetValue(Canvas.TopProperty, i * unitHeight);
                        canvas.Children.Add(lbl);
                    }
                }
            }
        }
    }
}

using AirMonitor.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                map.OnReloadMap();
            }
        }


        /// <summary>
        /// 无人机列表。
        /// </summary>
        public ObservableCollection<UavMarker3D> UavCollection
        {
            get { return (ObservableCollection<UavMarker3D>)GetValue(UavCollectionProperty); }
            set { SetValue(UavCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UavCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UavCollectionProperty =
            DependencyProperty.Register("UavCollection", typeof(ObservableCollection<UavMarker3D>), typeof(Map3D), new PropertyMetadata(null, new PropertyChangedCallback(OnUavCollectionChanged)));

        private static void OnUavCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Map3D map)
            {
                map.OnUavCollectionChanged(e);
            }
        }



        /// <summary>
        /// 方块列表。
        /// </summary>
        public ObservableCollection<BlockMarker3D> BlockCollection
        {
            get { return (ObservableCollection<BlockMarker3D>)GetValue(BlockCollectionProperty); }
            set { SetValue(BlockCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BlockCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BlockCollectionProperty =
            DependencyProperty.Register("BlockCollection", typeof(ObservableCollection<BlockMarker3D>), typeof(Map3D), new PropertyMetadata(null, new PropertyChangedCallback(OnBlockCollectionChanged)));

        private static void OnBlockCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Map3D map)
            {
                map.OnBlockCollectionChanged(e);
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
                    var longer = Math.Sqrt(img.Width * img.Width + img.Height * img.Height);
                    Camera.Position = Point3D.Parse(string.Format("0 {0} {1}", longer, longer));
                    Camera.UpDirection = Vector3D.Parse("0,1,0");
                    Camera.LookDirection = Vector3D.Parse(string.Format("0 -{0} -{1}", longer, longer + img.Height / 2));
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
                var diff = max;
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
                return diff == 0 ? double.NaN : (img.Height / 2 - img.Height / diff * (lat - min));
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
                return diff == 0 ? double.NaN : (img.Width / diff * (lng - min) - img.Width / 2);
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
                Dispatcher.Invoke(() =>
                {
                    var block = BlockGroup.Children.ToArray();
                    if (block != null && block.Any())
                    {
                        foreach (var item in block)
                        {
                            InitGeometryModel3D(item as GeometryModel3D);
                        }
                    }
                });
            }
        }

        private void OnUavCollectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ObservableCollection<UavMarker3D> oldUav)
            {
                oldUav.CollectionChanged -= Uav_CollectionChanged;
            }

            if (e.NewValue is ObservableCollection<UavMarker3D> newUav)
            {
                newUav.CollectionChanged += Uav_CollectionChanged;
                OnReloadUav();
            }
        }

        private void OnReloadUav()
        {
            UavGroup.Children.Clear();
            var uav = UavCollection.ToArray();//copy
            if (uav != null && MapBound != null)
            {
                foreach (var item in uav)
                {
                    UavGroup.Children.Add(CreateUav(item));
                }
            }
        }

        private Model3D CreateUav(UavMarker3D item)
        {
            var h = HeightConvert(item.Bound.Min.Height);
            var x1 = LngConvert(item.Bound.Min.Lng);
            var y1 = LatConvert(item.Bound.Min.Lat);
            var x2 = LngConvert(item.Bound.Max.Lng);
            var y2 = LatConvert(item.Bound.Max.Lat);
            var geometry = View3D.FindResource("UavGeometry") as MeshGeometry3D;
            geometry.Positions = Point3DCollection.Parse(string.Format("{1} {0} {2},{1} {0} {4},{3} {0} {4},{3} {0} {2}", h, x1, y1, x2, y2));
            var result = new GeometryModel3D(geometry, View3D.FindResource("UavMaterial") as Material);
            result.SetValue(MapMarker3D.MapMarkerProperty, item);
            return result;
        }

        private void Uav_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is UavMarker3D uav)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    UavGroup.Children.Add(CreateUav(uav));
                                });
                            }
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    Dispatcher.Invoke(() => UavGroup.Children.Clear());
                    break;
            }
        }

        private void OnBlockCollectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ObservableCollection<BlockMarker3D> oldBlock)
            {
                oldBlock.CollectionChanged -= Block_CollectionChanged;
            }
            if (e.NewValue is ObservableCollection<BlockMarker3D> newBlock)
            {
                newBlock.CollectionChanged += Block_CollectionChanged;
                OnReloadBlock();
            }
        }

        private void OnReloadBlock()
        {
            BlockGroup.Children.Clear();
            var block = BlockCollection.ToArray();//copy
            if (block != null && MapBound != null)
            {
                foreach (var item in block)
                {
                    BlockGroup.Children.Add(CreateBlock(item));
                }
            }
        }

        private Model3D CreateBlock(BlockMarker3D item)
        {
            var geometry = View3D.FindResource("BlockGeometry") as MeshGeometry3D;
            var material = View3D.FindResource("BlockMaterial") as DiffuseMaterial;
            var result = new GeometryModel3D(geometry.Clone(), material.Clone());
            result.SetValue(MapMarker3D.MapMarkerProperty, item);
            InitGeometryModel3D(result);
            return result;
        }

        private void InitGeometryModel3D(GeometryModel3D model3D)
        {
            if (model3D.GetValue(MapMarker3D.MapMarkerProperty) is BlockMarker3D item)
            {
                var x1 = LngConvert(item.Bound.Min.Lng);
                var z2 = LatConvert(item.Bound.Min.Lat);
                var y1 = HeightConvert(item.Bound.Min.Height);
                var x2 = LngConvert(item.Bound.Max.Lng);
                var z1 = LatConvert(item.Bound.Max.Lat);
                var y2 = y1 + x2 - x1;
                var geometry = model3D.Geometry as MeshGeometry3D;
                var sharpFormat = " {0} {4} {2},{0} {4} {5},{3} {4} {5},{3} {4} {2}," +
                                  " {0} {1} {5},{0} {1} {2},{3} {1} {2},{3} {1} {5}," +
                                  " {3} {4} {5},{3} {1} {5},{3} {1} {2},{3} {4} {2}," +
                                  " {0} {4} {2},{0} {1} {2},{0} {1} {5},{0} {4} {5}," +
                                  " {3} {4} {2},{3} {1} {2},{0} {1} {2},{0} {4} {2}," +
                                  " {0} {4} {5},{0} {1} {5},{3} {1} {5},{3} {4} {5} ";
                geometry.Positions = Point3DCollection.Parse(string.Format(sharpFormat, x1, y1, z1, x2, y2, z2));
                var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(item.Color);
                color.A = (byte)(item.Opacity * 255);
                var material = model3D.Material as DiffuseMaterial;
                if (material.Brush is RadialGradientBrush b)
                {
                    (b.GradientStops[0] as GradientStop).Color = color;
                }
            }

        }

        private void Block_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is BlockMarker3D block)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    BlockGroup.Children.Add(CreateBlock(block));
                                });
                            }
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    Dispatcher.Invoke(() => BlockGroup.Children.Clear());
                    break;
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
                var unitHeightValue = (maxHeight) / lines;
                for (int i = 0; i < 5; i++)
                {
                    canvas.Children.Add(new Line() { Stroke = color, X1 = 0, Y1 = i * unitHeight, X2 = canvas.Width, Y2 = i * unitHeight });
                    if (i > 0)
                    {
                        var lbl = new Label() { Foreground = color, Content = unitHeightValue * (lines - i) + "m" };
                        lbl.SetValue(Canvas.TopProperty, i * unitHeight);
                        canvas.Children.Add(lbl);
                    }
                }
            }
        }
    }
}

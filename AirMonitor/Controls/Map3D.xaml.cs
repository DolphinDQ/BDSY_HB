using AirMonitor.Core;
using System;
using System.Collections.Generic;
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

        // Using a DependencyProperty as the backing store for MaxLat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLatProperty =
            DependencyProperty.Register("MaxLat", typeof(double), typeof(Map3D), new PropertyMetadata(1,
                new PropertyChangedCallback(OnBoundChanged)));


        // Using a DependencyProperty as the backing store for MinLat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinLatProperty =
            DependencyProperty.Register("MinLat", typeof(double), typeof(Map3D), new PropertyMetadata(0,
                new PropertyChangedCallback(OnBoundChanged)));

        // Using a DependencyProperty as the backing store for MaxLng.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLngProperty =
            DependencyProperty.Register("MaxLng", typeof(double), typeof(Map3D), new PropertyMetadata(1,
                new PropertyChangedCallback(OnBoundChanged)));

        // Using a DependencyProperty as the backing store for MinLng.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinLngProperty =
            DependencyProperty.Register("MinLng", typeof(double), typeof(Map3D), new PropertyMetadata(0,
                new PropertyChangedCallback(OnBoundChanged)));

        private static void OnBoundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Map3D m)
            {
                m.OnBoundChanged();
            }
        }
        #endregion

        public Map3D()
        {
            InitializeComponent();
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
            }
        }

        private void OnBoundChanged()
        {

        }
    }
}

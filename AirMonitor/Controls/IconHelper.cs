using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AirMonitor.Controls
{
    public static class IconHelper
    {
        public static Visual GetIcon(DependencyObject obj)
        {
            return (Visual)obj.GetValue(IconProperty);
        }

        public static void SetIcon(DependencyObject obj, Visual value)
        {
            obj.SetValue(IconProperty, value);
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached("Icon", typeof(Visual), typeof(IconHelper), new PropertyMetadata(null));




    }
}

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


        public static string GetTitle(DependencyObject obj)
        {
            return (string)obj.GetValue(TitleProperty);
        }

        public static void SetTitle(DependencyObject obj, string value)
        {
            obj.SetValue(TitleProperty, value);
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.RegisterAttached("Title", typeof(string), typeof(IconHelper), new PropertyMetadata(""));


        public static string GetSubTitle(DependencyObject obj)
        {
            return (string)obj.GetValue(SubTitleProperty);
        }

        public static void SetSubTitle(DependencyObject obj, string value)
        {
            obj.SetValue(SubTitleProperty, value);
        }

        // Using a DependencyProperty as the backing store for SubTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubTitleProperty =
            DependencyProperty.RegisterAttached("SubTitle", typeof(string), typeof(IconHelper), new PropertyMetadata(""));


        public static object GetStatus(DependencyObject obj)
        {
            return (object)obj.GetValue(StatusProperty);
        }

        public static void SetStatus(DependencyObject obj, object value)
        {
            obj.SetValue(StatusProperty, value);
        }

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.RegisterAttached("Status", typeof(object), typeof(IconHelper), new PropertyMetadata(null));




        public static int GetIconWidth(DependencyObject obj)
        {
            return (int)obj.GetValue(IconWidthProperty);
        }

        public static void SetIconWidth(DependencyObject obj, int value)
        {
            obj.SetValue(IconWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.RegisterAttached("IconWidth", typeof(int), typeof(IconHelper), new PropertyMetadata(0));




        public static int GetIconHeight(DependencyObject obj)
        {
            return (int)obj.GetValue(IconHeightProperty);
        }

        public static void SetIconHeight(DependencyObject obj, int value)
        {
            obj.SetValue(IconHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.RegisterAttached("IconHeight", typeof(int), typeof(IconHelper), new PropertyMetadata(0));





    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace AirMonitor.Controls
{
    public static class PopupHelper
    {

        public static List<Popup> GetPopupList(DependencyObject obj)
        {
            return (List<Popup>)obj.GetValue(PopupListProperty);
        }

        public static void SetPopupList(DependencyObject obj, List<Popup> value)
        {
            obj.SetValue(PopupListProperty, value);
        }

        // Using a DependencyProperty as the backing store for PopupList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupListProperty =
            DependencyProperty.RegisterAttached("PopupList", typeof(List<Popup>), typeof(PopupHelper), new PropertyMetadata(new List<Popup>()));


        public static Window GetAttachWindow(DependencyObject obj)
        {
            return (Window)obj.GetValue(AttachWindowProperty);
        }

        public static void SetAttachWindow(DependencyObject obj, Window value)
        {
            obj.SetValue(AttachWindowProperty, value);
        }

        // Using a DependencyProperty as the backing store for AttachWindow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttachWindowProperty =
            DependencyProperty.RegisterAttached("AttachWindow", typeof(Window), typeof(PopupHelper), new PropertyMetadata(null, new PropertyChangedCallback(OnAttachWindowChanged)), new ValidateValueCallback(OnValidateAttchWindow));

        private static bool OnValidateAttchWindow(object value)
        {
            return value is null || value is Window;
        }

        private static void OnAttachWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Popup popup)
            {
                if (e.NewValue is Window newWin)
                {
                    var list = GetPopupList(newWin);
                    if (!list.Contains(popup))
                    {
                        list.Add(popup);
                    }
                    newWin.LocationChanged -= Window_LocationChanged;
                    newWin.LocationChanged += Window_LocationChanged;
                }
                if (e.OldValue is Window oldWin)
                {
                    var list = GetPopupList(oldWin);
                    list.Remove(popup);
                    oldWin.LocationChanged -= Window_LocationChanged;
                }
            }
        }

        private static async void Window_LocationChanged(object sender, System.EventArgs e)
        {
            if (sender is Window window)
            {
                var list = GetPopupList(window);
                foreach (var item in list)
                {
                    if (item.IsOpen)
                    {
                        item.IsOpen = false;
                        await Task.Delay(500);
                        if (window.WindowState != WindowState.Minimized)
                        {
                            item.IsOpen = true;
                        }
                    }
                }
            }
        }
    }
}

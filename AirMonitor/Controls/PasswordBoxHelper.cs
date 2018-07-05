using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AirMonitor.Controls
{
    public static class PasswordBoxHelper
    {
        public static string GetPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordProperty, value);
        }

        // 绑定时，mode=twoway 
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBoxHelper), new PropertyMetadata("", new PropertyChangedCallback(OnPasswordChanged)));

        private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox password)
            {
                password.PasswordChanged -= Password_PasswordChanged;
                password.PasswordChanged += Password_PasswordChanged;
                var pass = e.NewValue?.ToString();
                if (password.Password != pass)
                {
                    password.Password = pass;
                }
            }
        }

        private static void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox password)
            {
                SetPassword(password, password.Password);
            }
        }

    }
}

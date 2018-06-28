using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AirMonitor.Converter
{
    class IsNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (!(parameter is bool a))
            {
                a = false;
            }

            var result = (value != null);
            if (value is int i) result = i == 0;
            if (value is double d) result = d == 0;
            if (value is float f) result = f == 0;
            if (value is decimal de) result = de == 0;
            if (value is long l) result = l == 0;
            if (value is short s) result = s == 0;
            if (value is char c) result = c == 0;
            return result == a;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

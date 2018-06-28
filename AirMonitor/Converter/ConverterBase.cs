using AirMonitor.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AirMonitor.Converter
{
    public abstract class ConverterBase : IValueConverter
    {
        public IResourceManager ResourceManager { get; }
        public ConverterBase()
        {
            ResourceManager = AppBootstrapper.Container.GetInstance(typeof(IResourceManager), null) as IResourceManager;
        }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}

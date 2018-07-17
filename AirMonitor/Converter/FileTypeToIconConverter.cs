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
    class FileTypeToIconConverter : ConverterBase
    {


        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((CloudFileType)value) == CloudFileType.Directory ? ResourceManager.Resource("appbar_folder") : ResourceManager.Resource("appbar_image_gallery");
        }


        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

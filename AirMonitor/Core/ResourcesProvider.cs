using AirMonitor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AirMonitor.Core
{
    class ResourceProvider : FrameworkElement, IResourceProvider
    {
        public string GetText(string key)
        {
            return (string)FindResource(key);
        }

        public object Resource(string key) => FindResource(key);
    }
}

using AirMonitor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AirMonitor.Map
{
    class MapProvider : IMapProvider
    {
        private WebBrowser Browser { get; set; }
        public void LoadMap(object mapContainer)
        {
            if (mapContainer is WebBrowser)
            {
                Browser = mapContainer as WebBrowser;
                Browser.Source = new Uri($"file:///{AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/")}Map/map.html");
            }
        }
    }
}

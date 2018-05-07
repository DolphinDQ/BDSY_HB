using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using Caliburn.Micro;
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
        private IEventAggregator m_eventAggregator;

        public MapProvider(IEventAggregator eventAggregator)
        {
            m_eventAggregator = eventAggregator;
        }

        private WebBrowser Browser { get; set; }
        public void LoadMap(object mapContainer)
        {
            if (mapContainer is WebBrowser)
            {
                Browser = mapContainer as WebBrowser;
                Browser.Source = new Uri($"file:///{AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/")}Map/map.html");
                Browser.LoadCompleted -= Browser_LoadCompleted;
                Browser.LoadCompleted += Browser_LoadCompleted;
            }
        }

        private void Browser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            m_eventAggregator.PublishOnUIThread(new EvtMapLoad()
            {
                Provider = this,
                Url = e.Uri.ToString(),
            });
        }
    }
}

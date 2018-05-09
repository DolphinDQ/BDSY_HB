using AirMonitor.EventArgs;
using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Map
{
    [ComVisible(true)]
    public class MapEventHandler
    {
        private IEventAggregator m_eventAggregator;

        public MapEventHandler(IEventAggregator eventAggregator)
        {
            m_eventAggregator = eventAggregator;
        }

        public void On(string name, string json)
        {
            switch (name)
            {
                case "pointConvert":
                    m_eventAggregator.PublishOnUIThread(JsonConvert.DeserializeObject<EvtMapPointConverted>(json));
                    break;
                default:
                    break;
            }
        }
    }
}

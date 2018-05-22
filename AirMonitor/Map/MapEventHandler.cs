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
                    On<EvtMapPointConverted>(json);
                    break;
                case "load":
                    On<EvtMapLoad>();
                    break;
                case "horizontalAspect":
                    On<EvtMapHorizontalAspect>(json);
                    break;
                case "verticalAspect":
                    On<EvtMapVerticalAspect>(json);
                    break;
                case "clearAspect":
                    On<EvtMapClearAspect>();
                    break;
                case "selectAnalysisArea":
                    On<EvtMapSelectAnalysisArea>(json);
                    break;
                case "clearAnalysisArea":
                    On<EvtMapClearAnalysisArea>();
                    break;
                //case "boundChanged":
                //    On<EvtMapBoundChanged>(json);
                //    break;
                default:
                    break;
            }
        }

        private void On<T>(string json = null)
           where T : new()
        {
            if (json == null)
            {
                m_eventAggregator.PublishOnBackgroundThread(new T());
            }
            else
            {
                m_eventAggregator.PublishOnBackgroundThread(JsonConvert.DeserializeObject<T>(json));
            }
        }

    }
}

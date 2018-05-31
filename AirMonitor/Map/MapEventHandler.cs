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
            if (Enum.TryParse<MapEvents>(name, out var evt))
            {
                switch (evt)
                {
                    case MapEvents.pointConvert:
                        On<EvtMapPointConverted>(json);
                        break;
                    case MapEvents.load:
                        On<EvtMapLoad>();
                        break;
                    case MapEvents.horizontalAspect:
                        On<EvtMapHorizontalAspect>(json);
                        break;
                    case MapEvents.verticalAspect:
                        On<EvtMapVerticalAspect>(json);
                        break;
                    case MapEvents.clearAspect:
                        On<EvtMapClearAspect>();
                        break;
                    case MapEvents.selectAnalysisArea:
                        On<EvtMapSelectAnalysisArea>(json);
                        break;
                    case MapEvents.clearAnalysisArea:
                        On<EvtMapClearAnalysisArea>();
                        break;
                    case MapEvents.savePoints:
                        On<EvtMapSavePoints>(json);
                        break;
                    case MapEvents.boundChanged:
                        On<EvtMapBoundChanged>(json);
                        break;
                    case MapEvents.blockChanged:
                        On<EvtMapBlockChanged>(json);
                        break;
                    case MapEvents.uavChanged:
                        On<EvtMapUavChanged>(json);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                this.Warn("nuknow map event :{0} ->{1}", name, json);
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

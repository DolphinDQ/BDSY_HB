using AirMonitor.Interfaces;
using Caliburn.Micro;
using AirMonitor.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirMonitor.EventArgs;

namespace AirMonitor.ViewModels
{
    class MapViewModel : Screen,
        IHandle<EvtMapLoad>
    {
        private IMapProvider m_map;
        private IEventAggregator m_eventAggregator;
        public object MapContainer { get; set; }


        public MapViewModel(IMapProvider map, IEventAggregator eventAggregator)
        {
            m_map = map;
            m_eventAggregator = eventAggregator;
            m_eventAggregator.Subscribe(this);
        }

        public override void TryClose(bool? dialogResult = null)
        {
            base.TryClose(dialogResult);
            m_eventAggregator.Unsubscribe(this);
        }


        public void OnMapContainerChanged()
        {
            if (MapContainer != null)
            {
                m_map.LoadMap(MapContainer);
            }
        }

        public void Handle(EvtMapLoad message)
        {
            m_map.MapInitMenu(false);
        }
    }
}

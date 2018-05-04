using AirMonitor.Interfaces;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class MapViewModel : Screen
    {
        private IMapProvider m_mapProvider;

        public MapViewModel(IMapProvider mapProvider)
        {
            m_mapProvider = mapProvider;
        }

        public object MapContainer { get; set; }

        public void OnMapContainerChanged()
        {
            m_mapProvider.LoadMap(MapContainer);
        }
    }
}

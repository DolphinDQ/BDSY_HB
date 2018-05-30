using AirMonitor.Controls;
using AirMonitor.Interfaces;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirMonitor.Map;
using AirMonitor.EventArgs;

namespace AirMonitor.ViewModels
{
    class Map3DViewModel : Screen
    {

        public Map3DBound MapBound { get; set; } = new Map3DBound()
        {
            Max = new Map3DPoint()
            {
                Lat = 23,
                Lng = 112,
                Height = 155,
            },
            Min = new Map3DPoint()
            {
                Lat = 24,
                Lng = 120,
                Height = 100,
            }
        };

        public ObservableCollection<UavMarker3D> UavList { get; set; }

        public ObservableCollection<BlockMarker3D> BlockList { get; set; }

        public IMapView MapView { get; set; }

        public void OnMapViewChanged()
        {
            if (MapView != null)
            {
                var bound = MapView.MapProvider.Subscribe<EvtMapBoundChanged>(MapEvents.boundChanged, true)?.bound;
                if (bound != null)
                {
                    MapBound = new Map3DBound()
                    {
                        Max = new Map3DPoint()
                        {
                            Lat = bound.ne.lat,
                            Lng = bound.ne.lng,
                            Height = 1000,
                        },
                        Min = new Map3DPoint()
                        {
                            Lat = bound.sw.lat,
                            Lng = bound.ne.lng,
                            Height = 0
                        }
                    };
                }
            }
        }

    }
}

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
using AirMonitor.Config;

namespace AirMonitor.ViewModels
{
    class Map3DViewModel : Screen,
        //IHandle<EvtMapBlockChanged>,
        IHandle<EvtMapBoundChanged>
    //IHandle<EvtMapUavChanged>
    {
        private IEventAggregator m_eventAggregator;

        public Map3DViewModel(IConfigManager config, IEventAggregator eventAggregator)
        {
            Setting = config.GetConfig<AirStandardSetting>();
            eventAggregator.Subscribe(this);
            m_eventAggregator = eventAggregator;
        }

        public override void TryClose(bool? dialogResult = null)
        {
            base.TryClose(dialogResult);
            m_eventAggregator.Subscribe(this);
        }

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

        public ObservableCollection<UavMarker3D> UavList { get; set; } = new ObservableCollection<UavMarker3D>();

        public ObservableCollection<BlockMarker3D> BlockList { get; set; } = new ObservableCollection<BlockMarker3D>();

        public IMapView MapView { get; set; }

        public AirStandardSetting Setting { get; }

        public MapBound Bound { get; private set; }
        public MapBlock[] Blocks { get; private set; }
        public MapUav[] Uavs { get; private set; }

        public void OnBoundChanged()
        {
            if (Bound != null)
            {
                MapBound = new Map3DBound()
                {
                    Max = new Map3DPoint()
                    {
                        Lat = Bound.ne.lat,
                        Lng = Bound.ne.lng,
                        Height = Setting.MaxAltitude,
                    },
                    Min = new Map3DPoint()
                    {
                        Lat = Bound.sw.lat,
                        Lng = Bound.sw.lng,
                        Height = Setting.CorrectAltitude
                    }
                };
                var b = BlockList;
                BlockList = null;
                BlockList = b;
            }
        }


        //public async void OnUavsChanged()
        //{
        //    IEnumerable<MapUav> uavs = Uavs;
        //    await Task.Delay(200);
        //    OnUIThread(() => UavList.Clear());
        //    if (uavs != Uavs) return;
        //    if (Uavs != null && Uavs.Any() && Bound != null)
        //    {
        //        uavs = uavs.Where(o =>
        //                 o.lat > Bound.sw.lat &&
        //                 o.lng > Bound.sw.lng &&
        //                 o.lat < Bound.ne.lat &&
        //                 o.lng < Bound.ne.lng);
        //        if (!uavs.Any()) return;
        //        OnUIThread(() =>
        //        {
        //            foreach (var item in uavs)
        //            {
        //                UavList.Add(new UavMarker3D()
        //                {
        //                    Name = item.name,
        //                    Bound = new Map3DBound()
        //                    {
        //                        Max = new Map3DPoint()
        //                        {
        //                            Lat= item.lat
        //                        },
        //                        Min = new Map3DPoint() { }
        //                    },
        //                });
        //            }
        //        });
        //    }
        //}


        //public async void OnBlocksChanged()
        //{
        //    if (Blocks != null && Blocks.Any())
        //    {
        //        var len = Blocks.Length;
        //        await Task.Delay(200);
        //        if (len < Blocks.Length) return;
        //        BlockList.Clear();
        //        foreach (var item in Blocks)
        //        {
        //            var maxHeight = item.points.Max(o => o.hight) + 1;
        //            var minHeight = item.points.Min(o => o.hight) - 1;
        //            BlockList.Add(new BlockMarker3D()
        //            {
        //                Bound = new Map3DBound()
        //                {
        //                    Max = new Map3DPoint()
        //                    {
        //                        Height = maxHeight,
        //                        Lat = item.ne.lat,
        //                        Lng = item.ne.lng,
        //                    },
        //                    Min = new Map3DPoint()
        //                    {
        //                        Height = minHeight,
        //                        Lat = item.sw.lat,
        //                        Lng = item.sw.lng,
        //                    }
        //                },
        //                Color = item.color,
        //                Opacity = item.opacity,
        //            });
        //        }
        //    }
        //}

        //public void Handle(EvtMapBlockChanged message)
        //{
        //    Blocks = message.blocks;
        //}

        //public void Handle(EvtMapUavChanged message)
        //{
        //    Uavs = message.uav;
        //}


        public async void OnMapViewChanged()
        {
            var view = MapView;
            await Task.Delay(200);
            if (view != MapView) return;
            if (view != null)
            {
                Bound = view.MapProvider.Subscribe<EvtMapBoundChanged>(MapEvents.boundChanged, true)?.bound;
                //Blocks = MapView.MapProvider.Subscribe<EvtMapBlockChanged>(MapEvents.blockChanged, true)?.blocks;
                //Uavs = MapView.MapProvider.Subscribe<EvtMapUavChanged>(MapEvents.uavChanged, true)?.uav;
            }
        }

        public void Handle(EvtMapBoundChanged message)
        {
            Bound = message.bound;
        }

    }
}

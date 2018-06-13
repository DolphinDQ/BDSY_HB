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
        IHandle<EvtMapBoundChanged>,
        IHandle<EvtMapPointConverted>
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
            }
        }

        public async void OnMapViewChanged()
        {
            var view = MapView;
            await Task.Delay(200);
            if (view != MapView) return;
            if (view != null)
            {
                Bound = view.MapProvider.Subscribe<EvtMapBoundChanged>(MapEvents.boundChanged, true)?.bound;
                var samples = MapView.Samples.ToArray();
                OnUIThread(() =>
                {
                    foreach (var item in samples)
                    {
                        BlockList.Add(SampleToBlock3D(item));
                    }
                });
            }
        }

        public override void Refresh()
        {
            BlockList.Clear();
            OnMapViewChanged();
        }

        public void Handle(EvtMapBoundChanged message)
        {
            Bound = message.bound;
        }

        public void Handle(EvtMapPointConverted message)
        {
            var sample = MapView.Samples.FirstOrDefault(o => o.GetHashCode() == message.Seq);
            if (sample != null)
            {
                OnUIThread(() => BlockList.Add(SampleToBlock3D(sample)));
            }
        }

        private double GetSampleValue(EvtAirSample sample)
            => (double)typeof(EvtAirSample).GetProperty(MapView.DataName.Item1).GetValue(sample);

        private BlockMarker3D SampleToBlock3D(EvtAirSample sample)
        {
            var blockSize = 0.00001;//地图比例大约1米
            var color = MapView.MapGridOptions.GetColor(GetSampleValue(sample));
            return new BlockMarker3D()
            {
                Bound = new Map3DBound()
                {
                    Max = new Map3DPoint()
                    {
                        Height = sample.hight - Setting.CorrectAltitude + 1,
                        Lat = sample.ActualLat + blockSize,
                        Lng = sample.ActualLng + blockSize,
                    },
                    Min = new Map3DPoint()
                    {
                        Height = sample.hight - Setting.CorrectAltitude - 1,
                        Lat = sample.ActualLat - blockSize,
                        Lng = sample.ActualLng - blockSize,
                    },

                },
                Color = color,
                Opacity = 1
            };
        }
    }
}

using AirMonitor.Controls;
using AirMonitor.Interfaces;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AirMonitor.Map;
using AirMonitor.EventArgs;
using AirMonitor.Config;

namespace AirMonitor.ViewModels
{
    class Map3DViewModel : Screen,
        IHandle<EvtSampling>,
        IHandle<EvtSetting>,
        IHandle<EvtMapBoundChanged>,
        IHandle<EvtMapPointConverted>
    {
        private IEventAggregator m_eventAggregator;
        private IConfigManager m_config;

        public Map3DViewModel(IConfigManager config, IEventAggregator eventAggregator)
        {
            m_config = config;
            //Setting = config.GetConfig<AirStandardSetting>();
            eventAggregator.Subscribe(this);
            m_eventAggregator = eventAggregator;
        }

        public override void TryClose(bool? dialogResult = null)
        {
            base.TryClose(dialogResult);
            m_eventAggregator.Unsubscribe(this);
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

        public AirStandardSetting Setting => MapView?.Save?.Standard;

        public double MapOpacity { get; set; } = 1d;

        public double WallHeight { get; set; } = 400;

        public double Angle { get; set; }

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
                        Height = Setting?.MaxAltitude??0,
                    },
                    Min = new Map3DPoint()
                    {
                        Lat = Bound.sw.lat,
                        Lng = Bound.sw.lng,
                        Height = Setting?.CorrectAltitude??0
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
                var samples = MapView.Save?.Samples;
                if (samples == null) return;
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
            var sample = MapView.Save?.Samples.FirstOrDefault(o => o.GetHashCode() == message.Seq);
            if (sample != null)
            {
                OnUIThread(() => BlockList.Add(SampleToBlock3D(sample)));
            }
        }

        private double GetSampleValue(EvtAirSample sample)
            => (double)typeof(EvtAirSample).GetProperty(MapView.MapGridOptions.pollutant?.Name).GetValue(sample);

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

        public void Handle(EvtSampling message)
        {
            switch (message.Status)
            {
                case SamplingStatus.Stop:
                    break;
                case SamplingStatus.Start:
                    break;
                case SamplingStatus.ClearAll:
                    Refresh();
                    break;
                case SamplingStatus.StartSim:
                    break;
                case SamplingStatus.StopSim:
                    break;
                default:
                    break;
            }
        }

        public void Handle(EvtSetting message)
        {
            //switch (message.Command)
            //{
            //    case SettingCommands.Changed:
            //        if (message.SettingObject is AirStandardSetting setting)
            //        {
            //            Setting = setting;
            //            OnBoundChanged();
            //            Refresh();
            //        }
            //        break;
            //    default:
            //        break;
            //}
        }

        public void TurnLeft()
        {
            if (Angle % 45 != 0)
            {
                Angle = ((int)Angle / 45) * 45;
            }
            else
            {
                Angle -= 45;
            }
        }

        public void TurnRight()
        {
            if (Angle % 45 != 0)
            {
                Angle = ((int)Angle / 45) * 45;
            }
            else
            {
                Angle += 45;
            }
        }

        //public void LayerUp()
        //{
        //    if (Setting.MaxAltitude > 20)
        //    {
        //        Setting.MaxAltitude -= 10;
        //        m_config.SaveConfig(Setting);
        //    }
        //}

        //public void LayerDown()
        //{
        //    if (Setting.MaxAltitude < 200)
        //    {
        //        Setting.MaxAltitude += 10;
        //        m_config.SaveConfig(Setting);
        //    }
        //}
    }
}

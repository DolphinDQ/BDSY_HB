using AirMonitor.EventArgs;
using AirMonitor.Map;
using AirMonitor.Interfaces;
using Caliburn.Micro;
using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AirMonitor.ViewModels
{
    public class MapViewModel : Screen, IHandle<EvtAirSample>, IHandle<EvtSampling>, IHandle<EvtMapLoad>, IHandle<EvtMapPointConverted>
    {
        private IMapProvider m_mapProvider;
        private IEventAggregator m_eventAggregator;
        private IResourceProvider m_resourceProvider;
        public object MapContainer { get; set; }
        public List<EvtAirSample> Samples { get; private set; }
        public List<EvtAirSample> InvalidSample { get; private set; }
        /// <summary>
        /// 数据名称列表，是采样数据的名称列表。
        /// </summary>
        public List<Tuple<string, string>> DataNameList { get; set; }
        /// <summary>
        /// 采样状态。
        /// </summary>
        public bool Sampling { get; private set; }
        /// <summary>
        /// 地图已经加载。
        /// </summary>
        public bool MapLoad { get; private set; }
        /// <summary>
        /// 地图网格参数。
        /// </summary>
        public MapGridOptions MapGridOptions { get; private set; }
        /// <summary>
        /// 显示无人机路径。
        /// </summary>
        public bool ShowUavPath { get; set; } = true;
        /// <summary>
        /// 无人机跟踪
        /// </summary>
        public bool IsUavFocus { get; set; } = true;

        public MapViewModel(
            IEventAggregator eventAggregator,
            IMapProvider mapProvider,
            IResourceProvider resourceProvider)
        {
            m_mapProvider = mapProvider;
            m_eventAggregator = eventAggregator;
            m_resourceProvider = resourceProvider;
            m_eventAggregator.Subscribe(this);
            DataNameList = new List<Tuple<string, string>>(new[] {
                Tuple.Create(nameof(EvtAirSample.temp),resourceProvider.GetText("T_Temperature")),
                Tuple.Create(nameof(EvtAirSample.humi),resourceProvider.GetText("T_Humidity")),
                Tuple.Create(nameof(EvtAirSample.voc),resourceProvider.GetText("T_VOC")),
                Tuple.Create(nameof(EvtAirSample.co),resourceProvider.GetText("T_CO")),
                Tuple.Create(nameof(EvtAirSample.so2),resourceProvider.GetText("T_SO2")),
                Tuple.Create(nameof(EvtAirSample.no2),resourceProvider.GetText("T_NO2")),
                Tuple.Create(nameof(EvtAirSample.o3), resourceProvider.GetText("T_O3")),
                Tuple.Create(nameof(EvtAirSample.pm25), resourceProvider.GetText("T_PM2_5")),
                Tuple.Create(nameof(EvtAirSample.pm10), resourceProvider.GetText("T_PM10")),
            });
            Samples = new List<EvtAirSample>();
            MapGridOptions = new MapGridOptions();
        }

        public override void TryClose(bool? dialogResult = null)
        {
            m_eventAggregator.Unsubscribe(this);
            base.TryClose(dialogResult);
        }

        public void OnMapContainerChanged()
        {
            m_mapProvider.LoadMap(MapContainer);
        }
        /// <summary>
        /// 接收环境检测数据。
        /// </summary>
        /// <param name="message"></param>
        public void Handle(EvtAirSample message)
        {
            if (Sampling)
            {
                if (message.lat == 0 || message.lon == 0)
                {
                    this.Warn("unknow sample location.");
                    InvalidSample.Add(message);
                    NotifyOfPropertyChange(nameof(InvalidSample));
                }
                else
                {
                    Samples.Add(message);
                    NotifyOfPropertyChange(nameof(Samples));
                    m_mapProvider.MapPointConvert(message.GetHashCode(), new[] { new MapPoint() { lat = message.GpsLat, lng = message.GpsLng } });
                }
            }
        }

        public void Handle(EvtSampling message)
        {
            switch (message.Status)
            {
                case SamplingStatus.Stop:
                    Sampling = false;
                    break;
                case SamplingStatus.Start:
                    Sampling = true;
                    break;
                case SamplingStatus.Clear:
                    Samples.Clear();
                    NotifyOfPropertyChange(nameof(Samples));
                    break;
                default:
                    break;
            }
        }

        public async void Handle(EvtMapLoad message)
        {
            await Task.Delay(1000);//地图加载延时一秒钟
            MapLoad = true;
        }

        private void OnUpdateUavPosition(EvtAirSample sample)
        {
            if (MapLoad)
            {
                var name = GetUavName(sample);
                if (!m_mapProvider.UavExist(name))
                {
                    var s = Samples.Where(o => o.ActualLat != 0 && o.ActualLng != 0).ToList();
                    var first = s.First();
                    m_mapProvider.UavAdd(new Uav { name = name, data = first, lat = first.ActualLat, lng = first.ActualLng });
                    foreach (var item in s)
                    {
                        m_mapProvider.UavMove(new Uav() { name = name, data = item, lat = item.ActualLat, lng = item.ActualLng });
                    }
                    m_mapProvider.GridInit(MapGridOptions);
                }
                else
                {
                    m_mapProvider.UavMove(new Uav { name = name, data = sample, lat = sample.ActualLat, lng = sample.ActualLng });
                    m_mapProvider.GridRefresh();
                    m_mapProvider.UavPath(name, ShowUavPath);
                }
                if (IsUavFocus)
                {
                    m_mapProvider.UavFocus(name);
                }
            }
        }

        private string GetUavName(EvtAirSample sample) => "default";

        public void RefreshMap()
        {
            MapLoad = false;
            m_mapProvider.LoadMap(MapContainer);
        }

        public void Test()
        {
            //Task.Factory.StartNew(() =>
            //{
            //    var random = new Random();
            //    var lat = 23.016791666666666667;
            //    var lng = 113.077023333333333333;
            //    do
            //    {
            //        OnUIThread(() =>
            //        {
            //            Handle(new EvtAirSample()
            //            {
            //                co = 60 + random.NextDouble() * 40,
            //                lat = lat -= 0.0001,
            //                lon = lng -= 0.0001
            //            });
            //        });
            //        Task.Delay(1000).Wait();
            //    } while (true);
            //});
            //m_mapProvider.Invoke("mapPointConvert", 1, JsonConvert.SerializeObject(new[] { new MapPoint() { lat = 23.016791666666666667, lng = 113.077023333333333333 } }));
        }

        public void Handle(EvtMapPointConverted message)
        {
            var sample = Samples.FirstOrDefault(o => o.GetHashCode() == message.Seq);
            if (sample != null)
            {
                var point = message.Points.FirstOrDefault();
                sample.ActualLat = point.lat;
                sample.ActualLng = point.lng;
                OnUpdateUavPosition(sample);
            }
        }
    }
}

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
using AirMonitor.Config;

namespace AirMonitor.ViewModels
{
    public class MapViewModel : Screen, IHandle<EvtAirSample>, IHandle<EvtSampling>, IHandle<EvtMapLoad>, IHandle<EvtMapPointConverted>
    {
        private IMapProvider m_mapProvider;
        private IEventAggregator m_eventAggregator;
        private IResourceManager m_res;
        private ISaveManager m_saveManager;
        private AirStandardSetting m_airStandard;

        public object MapContainer { get; set; }
        public List<EvtAirSample> Samples { get; private set; }
        public List<EvtAirSample> InvalidSamples { get; private set; }
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
        /// <summary>
        /// 污染物名称。
        /// </summary>
        public Tuple<string, string> DataName { get; set; }

        public MapViewModel(
            IEventAggregator eventAggregator,
            IMapProvider mapProvider,
            ISaveManager saveManager,
            IConfigManager configManager,
            IResourceManager res)
        {
            m_mapProvider = mapProvider;
            m_eventAggregator = eventAggregator;
            m_res = res;
            m_saveManager = saveManager;
            m_eventAggregator.Subscribe(this);
            m_airStandard = configManager.GetConfig<AirStandardSetting>();
            Samples = new List<EvtAirSample>();
            InvalidSamples = new List<EvtAirSample>();
            MapGridOptions = new MapGridOptions();
            DataNameList = new List<Tuple<string, string>>(new[] {
                Tuple.Create(nameof(EvtAirSample.temp),res.GetText("T_Temperature")),
                Tuple.Create(nameof(EvtAirSample.humi),res.GetText("T_Humidity")),
                Tuple.Create(nameof(EvtAirSample.voc),res.GetText("T_VOC")),
                Tuple.Create(nameof(EvtAirSample.co),res.GetText("T_CO")),
                Tuple.Create(nameof(EvtAirSample.so2),res.GetText("T_SO2")),
                Tuple.Create(nameof(EvtAirSample.no2),res.GetText("T_NO2")),
                Tuple.Create(nameof(EvtAirSample.o3), res.GetText("T_O3")),
                Tuple.Create(nameof(EvtAirSample.pm25), res.GetText("T_PM2_5")),
                Tuple.Create(nameof(EvtAirSample.pm10), res.GetText("T_PM10")),
            });
            DataName = DataNameList.First();
        }

        public override void TryClose(bool? dialogResult = null)
        {
            m_eventAggregator.Unsubscribe(this);
            base.TryClose(dialogResult);
        }

        public void OnDataNameChanged()
        {
            var pollutant = m_airStandard.Pollutant.FirstOrDefault(o => o.Name == DataName.Item1);
            if (pollutant != null)
            {
                MapGridOptions.maxValue = pollutant.MaxValue;
                MapGridOptions.minValue = pollutant.MinValue;
            }
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
                    InvalidSamples.Add(message);
                    NotifyOfPropertyChange(nameof(InvalidSamples));
                }
                else
                {
                    Samples.Add(message);
                    NotifyOfPropertyChange(nameof(Samples));
                    if (MapLoad)
                    {
                        m_mapProvider.MapPointConvert(message.GetHashCode(), new[] { new MapPoint() { lat = message.GpsLat, lng = message.GpsLng } });
                    }
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
                    //ClearSamples(true);
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
                    LoadHistoryData(name);
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

        private void LoadHistoryData(string name)
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

        public void OnMapLoadChanged()
        {
            if (Samples.Any())
            {
                LoadHistoryData(GetUavName(null));
            }
        }
        public void UavLocation()
        {
            if (MapLoad)
            {
                m_mapProvider.UavFocus(GetUavName(null));
            }
        }

        public void ClearSamples(bool focus = false)
        {
            if (Samples.Any() && (focus || MessageBox.Show(m_res.GetText("T_ClearSamplesWarning"), "", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
            {
                Samples.Clear();
                NotifyOfPropertyChange(nameof(Samples));
                InvalidSamples.Clear();
                NotifyOfPropertyChange(nameof(InvalidSamples));
                RefreshMap();
            }
        }

        public void SaveSamples()
        {
            if (!Samples.Any()) return;
            try
            {
                var file = m_saveManager.ShowSaveFileDialog();
                if (file == null) return;
                m_saveManager.Save(file, Samples);
            }
            catch (Exception e)
            {
                MessageBox.Show(m_res.GetText("T_SaveSamplesFailed") + e.Message);
            }

        }

        public void LoadSamples()
        {
            if (Sampling)
            {
                MessageBox.Show(m_res.GetText("T_LoadSamplesWarning"));
                return;
            }

            try
            {
                var file = m_saveManager.ShowOpenFileDialog();
                if (file != null)
                {
                    IEnumerable<EvtAirSample> samples = m_saveManager.Load<EvtAirSample[]>(file);
                    MessageBoxResult overwriteSample = MessageBoxResult.Yes;
                    if (Samples.Any())
                    {
                        overwriteSample = MessageBox.Show(m_res.GetText("T_LoadSamplesOverwriteWarning"), "", MessageBoxButton.YesNoCancel);
                        if (overwriteSample == MessageBoxResult.Yes)
                        {
                            samples = samples.OrderBy(o => o.RecordTime);
                            Samples.Clear();
                        }
                        if (overwriteSample == MessageBoxResult.No)
                        {
                            samples = samples.Concat(Samples).OrderBy(o => o.RecordTime).ToArray();
                            Samples.Clear();
                        }
                    }
                    else
                    {
                        samples = samples.OrderBy(o => o.RecordTime);
                    }
                    switch (overwriteSample)
                    {
                        case MessageBoxResult.None:
                        case MessageBoxResult.OK:
                        case MessageBoxResult.Cancel:
                            return;
                    }
                    Samples.AddRange(samples);
                    NotifyOfPropertyChange(nameof(Samples));
                    RefreshMap();

                }

            }
            catch (Exception e)
            {
                MessageBox.Show(m_res.GetText("T_LoadSamplesFailed") + e.Message);
            }

        }

        public void RefreshBlock()
        {
            m_mapProvider.GridInit(MapGridOptions);
            m_mapProvider.GridRefresh();
            m_mapProvider.UavPath(GetUavName(null), ShowUavPath);
        }
    }
}

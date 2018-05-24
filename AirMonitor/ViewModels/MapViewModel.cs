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
    public class MapViewModel : Screen, IMapView,
        IHandle<EvtAirSample>,
        IHandle<EvtSampling>,
        IHandle<EvtMapLoad>,
        IHandle<EvtMapPointConverted>,
        IHandle<EvtSetting>,
        IHandle<EvtMapHorizontalAspect>,
        IHandle<EvtMapVerticalAspect>,
        IHandle<EvtMapSelectAnalysisArea>,
        IHandle<EvtMapClearAnalysisArea>,
        IHandle<EvtMapSavePoints>,
        IHandle<EvtMapClearAspect>
    {
        private IMapProvider m_mapProvider;
        private IEventAggregator m_eventAggregator;
        private IResourceManager m_res;
        private IFactory m_factory;
        private ISaveManager m_saveManager;

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
        /// 开始数据分析。
        /// </summary>
        public bool EnableAnalysis { get; set; } = false;
        /// <summary>
        /// 污染物名称。
        /// </summary>
        public Tuple<string, string> DataName { get; set; }
        /// <summary>
        /// 地图提供者。
        /// </summary>
        public IMapProvider MapProvider => m_mapProvider;
        /// <summary>
        /// 属性框。
        /// </summary>
        public object PropertyPanel { get; set; }
        /// <summary>
        /// 比较框。
        /// </summary>
        public object ComparePanel { get; set; }

        public MapViewModel(
            IEventAggregator eventAggregator,
            IMapProvider mapProvider,
            ISaveManager saveManager,
            IConfigManager configManager,
            IFactory factory,
            IResourceManager res)
        {
            m_mapProvider = mapProvider;
            m_eventAggregator = eventAggregator;
            m_res = res;
            m_factory = factory;
            m_saveManager = saveManager;
            m_eventAggregator.Subscribe(this);
            var setting = configManager.GetConfig<AirStandardSetting>();
            Samples = new List<EvtAirSample>();
            InvalidSamples = new List<EvtAirSample>();
            MapGridOptions = new MapGridOptions()
            {
                pollutants = setting.Pollutant
            };
            DataNameList = new List<Tuple<string, string>>();
            foreach (var item in setting.Pollutant)
            {
                DataNameList.Add(Tuple.Create(item.Name, item.DisplayName));
            }
            DataName = DataNameList.First();
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

        public void OnMapLoadChanged()
        {
            if (Samples.Any())
            {
                LoadHistoryData(GetUavName(null));
            }
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
                case SamplingStatus.ClearAll:
                    ClearSamples(true);
                    break;
                default:
                    break;
            }
        }

        public void Handle(EvtMapLoad message)
        {
            MapLoad = true;
        }

        public void Handle(EvtSetting message)
        {
            if (message.Command == SettingCommands.Changed && message.SettingObject is AirStandardSetting setting)
            {
                MapGridOptions.pollutants = setting.Pollutant;
            }
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

        public void Handle(EvtMapHorizontalAspect message)
            => OnShowAnalysisPanel(message.blocks, AnalysisMode.Horizontal);

        public void Handle(EvtMapVerticalAspect message)
            => OnShowAnalysisPanel(message.blocks, AnalysisMode.Vertical);

        public void Handle(EvtMapClearAspect message)
        {
            if (PropertyPanel is SampleAnalysisViewModel)
            {
                EnableAnalysis = false;
            }
        }

        public void Handle(EvtMapSelectAnalysisArea message)
        {
            if (!(PropertyPanel is DynamicAnalysisViewModel view))
            {
                view = m_factory.Create<DynamicAnalysisViewModel>();
            }
            view.MapView = this;
            view.Bounds = message;
            PropertyPanel = view;
            EnableAnalysis = true;
        }

        public void Handle(EvtMapClearAnalysisArea message)
        {
            if (PropertyPanel is DynamicAnalysisViewModel)
            {
                EnableAnalysis = false;
            }
        }

        public void Handle(EvtMapSavePoints message)
        {
            OnSaveSamples(message.points.Cast<EvtAirSample>().ToArray());
        }

        private void OnSaveSamples(EvtAirSample[] airSamples)
        {
            OnUIThread(() =>
            {
                try
                {
                    var file = m_saveManager.ShowSaveFileDialog();
                    if (file == null) return;
                    m_saveManager.Save(file, airSamples);
                }
                catch (Exception e)
                {
                    MessageBox.Show(m_res.GetText("T_SaveSamplesFailed") + e.Message);
                }
            });
        }

        private void OnShowAnalysisPanel(MapBlock[] blocks, AnalysisMode mode)
        {
            if (!(PropertyPanel is SampleAnalysisViewModel view))
            {
                view = m_factory.Create<SampleAnalysisViewModel>();
            }
            view.MapView = this;
            view.Mode = mode;
            view.MapBlocks = blocks;
            PropertyPanel = view;
            EnableAnalysis = true;
        }

        public void OnEnableAnalysisChanged()
        {
            if (!EnableAnalysis && PropertyPanel is Screen s)
            {
                s.TryClose();
                PropertyPanel = null;
            }
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
            var first = s.FirstOrDefault();
            if (first == null) return;
            m_mapProvider.UavAdd(new Uav { name = name, data = first, lat = first.ActualLat, lng = first.ActualLng });
            foreach (var item in s)
            {
                m_mapProvider.UavMove(new Uav() { name = name, data = item, lat = item.ActualLat, lng = item.ActualLng });
            }
            m_mapProvider.GridInit(MapGridOptions);
            m_mapProvider.UavFocus(name);
            m_mapProvider.GridRefresh();
        }

        private string GetUavName(EvtAirSample sample) => "default";

        public void RefreshMap()
        {
            MapLoad = false;
            EnableAnalysis = false;
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

        public void UavLocation()
        {
            if (MapLoad)
            {
                m_mapProvider.UavFocus(GetUavName(null));
            }
        }

        public void ClearSamples(bool focus = false)
        {
            if (Samples.Any() && (focus || MessageBox.Show(m_res.GetText("T_ClearMapWarning"), "", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
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
            OnSaveSamples(Samples.ToArray());
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
            m_mapProvider.GridClear();
            m_mapProvider.GridRefresh();
            m_mapProvider.UavPath(GetUavName(null), ShowUavPath);
        }

    }
}

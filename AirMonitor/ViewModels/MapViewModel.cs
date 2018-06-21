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
        IHandle<EvtSampleSaving>,
        IHandle<EvtMapClearAspect>
    {
        private IEventAggregator m_eventAggregator;
        private IResourceManager m_res;
        private IFactory m_factory;
        private IConfigManager m_configManager;
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
        /// 地图提供者。
        /// </summary>
        public IMapProvider MapProvider { get; }
        /// <summary>
        /// 属性框。
        /// </summary>
        public object PropertyPanel { get; set; }

        public object Map3DFullPanel { get; set; }

        public bool Show3DView { get; set; }

        public bool Map3DFullScreen { get; set; }

        private void SetPropertyPanel(object obj)
        {
            if (!(obj is Screen) && obj != null)
            {
                return;
            }
            if (PropertyPanel is Screen s)
            {
                s.TryClose();
            }
            PropertyPanel = obj;
        }

        public object Map3DPanel { get; set; }

        public object CameraPanel { get; set; }

        public MapViewModel(
            IEventAggregator eventAggregator,
            IMapProvider mapProvider,
            ISaveManager saveManager,
            IConfigManager configManager,
            IFactory factory,
            IResourceManager res)
        {
            MapProvider = mapProvider;
            m_eventAggregator = eventAggregator;
            m_res = res;
            m_factory = factory;
            m_configManager = configManager;
            m_saveManager = saveManager;
            m_eventAggregator.Subscribe(this);
            var setting = configManager.GetConfig<AirStandardSetting>();
            Samples = new List<EvtAirSample>();
            InvalidSamples = new List<EvtAirSample>();
            MapGridOptions = new MapGridOptions()
            {
                pollutants = setting.Pollutant
            };
        }

        public override void TryClose(bool? dialogResult = null)
        {
            m_eventAggregator.Unsubscribe(this);
            base.TryClose(dialogResult);
        }

        public void OnMapContainerChanged()
        {
            MapProvider.LoadMap(MapContainer);
        }

        public void OnMapLoadChanged()
        {
            if (!MapLoad)
            {
                SetPropertyPanel(null);
            }
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
                        MapProvider.MapPointConvert(message.GetHashCode(), new[] { new MapPoint() { lat = message.GpsLat, lng = message.GpsLng } });
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
            if (MapLoad)
            {
                MapLoad = false;
            }
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
            if (PropertyPanel is AnalysisStaticViewModel)
            {
                SetPropertyPanel(null);
            }
        }

        public void Handle(EvtMapSelectAnalysisArea message)
        {
            if (!(PropertyPanel is AnalysisDynamicViewModel view))
            {
                view = m_factory.Create<AnalysisDynamicViewModel>();
            }
            view.MapView = this;
            view.Bounds = message;
            SetPropertyPanel(view);
        }

        public void Handle(EvtMapClearAnalysisArea message)
        {
            if (PropertyPanel is AnalysisDynamicViewModel)
            {
                SetPropertyPanel(null);
            }
        }


        public void Handle(EvtSampleSaving message)
        {
            if (message.Type == SaveType.LoadSamplesCompleted)
            {
                OnLoadSampleCompleted(message.Save.Samples);
            }
        }


        private void OnShowAnalysisPanel(MapBlock[] blocks, AnalysisMode mode)
        {
            if (!(PropertyPanel is AnalysisStaticViewModel view))
            {
                view = m_factory.Create<AnalysisStaticViewModel>();
            }
            view.MapView = this;
            view.Mode = mode;
            view.MapBlocks = blocks;
            SetPropertyPanel(view);
        }

        private bool m_mapRefreshDelay = false;

        private async void OnUpdateUavPosition(EvtAirSample sample)
        {
            if (MapLoad)
            {
                var name = GetUavName(sample);
                if (!MapProvider.UavExist(name))
                {
                    LoadHistoryData(name);
                }
                else
                {
                    MapProvider.UavMove(new MapUav { name = name, data = sample, lat = sample.ActualLat, lng = sample.ActualLng });
                    if (m_mapRefreshDelay) return;
                    m_mapRefreshDelay = true;
                    await Task.Delay(1000);
                    m_mapRefreshDelay = false;
                    MapProvider.GridRefresh();
                    MapProvider.UavPath(name);
                }
                //if (IsUavFocus)
                //{
                //    MapProvider.UavFocus(name);
                //}
            }
        }

        public async void OnMap3DFullScreenChanged()
        {
            if (Map3DFullScreen)
            {
                if (Map3DPanel != null)
                {
                    var p = Map3DPanel;
                    Map3DPanel = null;
                    await Task.Delay(100);
                    Map3DFullPanel = p;
                }
            }
            else
            {
                if (Map3DFullPanel != null)
                {
                    var p = Map3DFullPanel;
                    Map3DFullPanel = null;
                    await Task.Delay(100);
                    Map3DPanel = p;
                }
            }
        }

        private void LoadHistoryData(string name)
        {
            Show3DView = false;
            SetPropertyPanel(null);
            var s = Samples.Where(o => o.ActualLat != 0 && o.ActualLng != 0).ToList();
            var first = s.FirstOrDefault();
            if (first == null) return;
            MapProvider.UavAdd(new MapUav { name = name, data = s, lat = first.ActualLat, lng = first.ActualLng });
            MapProvider.GridInit(MapGridOptions);
            MapProvider.UavFocus(name);
            MapProvider.GridRefresh();
        }

        private string GetUavName(EvtAirSample sample) => "default";

        public void RefreshMap()
        {
            MapLoad = false;
            MapProvider.LoadMap(MapContainer);
        }

        public void Test()
        {
            m_eventAggregator.PublishOnBackgroundThread(new EvtSetting() { Command = SettingCommands.Request, SettingObject = m_configManager.GetConfig<CameraSetting>() });
            //Show3D(true);
        }

        public void OnShow3DViewChanged() => Show3D(Show3DView);

        public void Show3D(bool display)
        {
            if (Map3DPanel is Screen s)
            {
                s.TryClose();
            }
            if (display)
            {
                var view = m_factory.Create<Map3DViewModel>();
                view.MapView = this;
                if (Map3DFullScreen)
                {
                    Map3DFullPanel = view;
                }
                else
                {
                    Map3DPanel = view;
                }
            }
            else
            {
                Map3DPanel = null;
                Map3DFullPanel = null;
            }
        }

        public void UavLocation()
        {
            if (MapLoad)
            {
                MapProvider.UavFocus(GetUavName(null));
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

        private void OnLoadSampleCompleted(IEnumerable<EvtAirSample> samples)
        {
            try
            {
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
                if (overwriteSample != MessageBoxResult.Cancel)
                {
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
            //MapGridOptions.Reload();
            MapProvider.GridInit(MapGridOptions);
            MapProvider.GridClear();
            MapProvider.GridRefresh();
            MapProvider.UavPath(GetUavName(null));
            if (Show3DView)
            {
                if (Map3DFullPanel is Screen s1)
                {
                    s1.Refresh();
                }
                else if (Map3DPanel is Screen s2)
                {
                    s2.Refresh();
                }
            }
            if (PropertyPanel is Screen s)
            {
                s.Refresh();
            }
        }

        public void SaveSamples()
        {
            if (!Samples.Any()) return;
            m_eventAggregator.PublishOnBackgroundThread(new EvtSampleSaving()
            {
                Type = SaveType.SaveSamples,
                Save = new AirSamplesSave()
                {
                    Samples = Samples.ToArray(),
                }
            });
        }

        public void LoadSamples()
        {
            if (Sampling)
            {
                MessageBox.Show(m_res.GetText("T_LoadSamplesWarning"));
                return;
            }
            m_eventAggregator.BeginPublishOnUIThread(new EvtSampleSaving() { Type = SaveType.LoadSamples });
        }


    }
}

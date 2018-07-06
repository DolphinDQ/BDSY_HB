using AirMonitor.Interfaces;
using Caliburn.Micro;
using AirMonitor.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirMonitor.EventArgs;
using AirMonitor.Config;
using System.Windows;

namespace AirMonitor.ViewModels
{
    class MapViewModel : Screen, IMapView,
        IHandle<EvtMapLoad>,
        IHandle<EvtSampleSaving>,
        IHandle<EvtMapVerticalAspect>,
        IHandle<EvtMapHorizontalAspect>,
        IHandle<EvtMapClearAspect>,
        IHandle<EvtSampling>
    {
        private IMapProvider MapProvider { get; }
        /// <summary>
        /// 默认配置。
        /// </summary>
        public AirStandardSetting DefaultStandard { get; }
        public object MapContainer { get; set; }
        public bool MapLoad { get; set; }
        public AirSamplesSave Save { get; set; }
        public MapGridOptions Option { get; set; }
        private ISaveManager m_saveManager;
        private IFactory m_factory;
        private IEventAggregator m_eventAggregator;

        public MapViewModel(IMapProvider map,
            IConfigManager config,
            ISaveManager saveManager,
            IFactory factory,
            IEventAggregator eventAggregator)
        {
            MapProvider = map;
            DefaultStandard = config.GetConfig<AirStandardSetting>();
            Option = new MapGridOptions();
            Option.settings = DefaultStandard;
            Option.pollutant = DefaultStandard.Pollutant.First();
            m_saveManager = saveManager;
            m_factory = factory;
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
                MapLoad = false;
                MapProvider.LoadMap(MapContainer);
            }
        }

        public void Handle(EvtMapLoad message)
        {
            MapProvider.MapInitMenu(false);
            MapLoad = true;
            OnLoadHistory();
        }

        private void OnLoadHistory()
        {
            if (Save != null && Save.Samples != null && Save.Samples.Any())
            {
                SetPropertyPanel(null);
                var first = Save.Samples.First();
                var name = GetUavName();
                MapProvider.UavAdd(new MapUav { name = name, data = Save.Samples, lat = first.ActualLat, lng = first.ActualLng });
                MapProvider.GridInit(Option);
                MapProvider.UavFocus(name);
                MapProvider.GridRefresh();
            }
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
                    Save = null;
                    RefreshMap();
                    break;
                case SamplingStatus.StartSim:
                    break;
                case SamplingStatus.StopSim:
                    break;
                default:
                    break;
            }

        }

        public void Handle(EvtSampleSaving message)
        {
            switch (message.Type)
            {
                case SaveType.SaveSamplesRequest:
                    OnSaveSample();
                    break;
                case SaveType.LoadSamplesRequest:
                    m_eventAggregator.PublishOnBackgroundThread(new EvtSampleSaving() { Type = SaveType.LoadSamples });
                    break;
                case SaveType.LoadSamplesCompleted:
                    OnLoadSample(message.Save);
                    break;

            }
        }

        private string GetUavName(EvtAirSample sample = null) => sample?.UavName ?? new EvtAirSample().UavName;

        public void RefreshMap() => OnMapContainerChanged();

        public void RefreshBlock()
        {
            try
            {
                MapProvider.GridInit(Option);
                MapProvider.GridClear();
                MapProvider.GridRefresh();
                MapProvider.UavPath(GetUavName());
                MapProvider.UavFocus(GetUavName());
            }
            catch (Exception e)
            {
                this.Warn("RefreshBlock error:{0}", e.Message);
                this.Error(e);
            }
        }

        private void OnLoadSample(AirSamplesSave save)
        {
            if (Save != null && save.Samples != null && save.Samples.Any())
            {
                var res = MessageBox.Show("是否要覆盖当前样本？", "注意", MessageBoxButton.YesNoCancel);
                switch (res)
                {
                    case MessageBoxResult.Yes://override
                        Save = save;
                        break;
                    case MessageBoxResult.No:
                        Save.Samples = Save.Samples.Concat(save.Samples).ToArray();
                        Save.Standard = save.Standard ?? Save.Standard;
                        break;
                    default:
                        return;
                }
            }
            else
            {
                Save = save;
            }
            Save.Standard = Save.Standard ?? DefaultStandard;
            RefreshMap();
            RefreshOverlayPanel();
        }

        private void OnSaveSample()
        {
            if (Save != null)
            {
                m_eventAggregator.PublishOnBackgroundThread(new EvtSampleSaving() { Type = SaveType.SaveSamples, Save = Save });
            }
        }

        #region IMapView

        object IMapView.MapContainer => MapContainer;

        bool IMapView.MapLoad => MapLoad;

        IMapProvider IMapView.MapProvider => MapProvider;

        List<EvtAirSample> IMapView.Samples => Save?.Samples?.ToList();

        bool IMapView.Sampling => false;

        MapGridOptions IMapView.MapGridOptions => Option;

        #endregion


        #region 浮动窗口

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

        /// <summary>
        /// 刷新覆盖在地图上的控件
        /// </summary>
        private void RefreshOverlayPanel()
        {
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

        public object Map3DPanel { get; set; }

        public void Handle(EvtMapClearAspect message)
        {
            if (PropertyPanel is AnalysisStaticViewModel)
            {
                SetPropertyPanel(null);
            }
        }

        public void Handle(EvtMapHorizontalAspect message)
          => OnShowAnalysisPanel(message.blocks, AnalysisMode.Horizontal);

        public void Handle(EvtMapVerticalAspect message)
            => OnShowAnalysisPanel(message.blocks, AnalysisMode.Vertical);

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

        #endregion
    }
}

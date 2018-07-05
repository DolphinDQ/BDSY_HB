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
    class MapViewModel : Screen,
        IHandle<EvtMapLoad>,
        IHandle<EvtSampleSaving>,
        IHandle<EvtSampling>
    {
        private IMapProvider MapProvider { get; }
        /// <summary>
        /// 默认配置。
        /// </summary>
        public AirStandardSetting DefaultStandard { get; }
        public object MapContainer { get; set; }
        public AirSamplesSave Save { get; set; }
        public MapGridOptions Option { get; set; }
        private ISaveManager m_saveManager;
        private IEventAggregator m_eventAggregator;

        public MapViewModel(IMapProvider map,
            IConfigManager config,
            ISaveManager saveManager,
            IEventAggregator eventAggregator)
        {
            MapProvider = map;
            DefaultStandard = config.GetConfig<AirStandardSetting>();
            Option = new MapGridOptions();
            Option.settings = DefaultStandard;
            Option.pollutant = DefaultStandard.Pollutant.First();
            m_saveManager = saveManager;
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
                MapProvider.LoadMap(MapContainer);
            }
        }

        public void Handle(EvtMapLoad message)
        {
            MapProvider.MapInitMenu(false);
            OnLoadHistory();
        }

        private void OnLoadHistory()
        {
            if (Save != null && Save.Samples != null && Save.Samples.Any())
            {
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
                    RefreshBlock();
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
                MapProvider.UavPath(GetUavName(null));
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
                Save.Standard = Save.Standard ?? DefaultStandard;
                RefreshMap();
            }
            else
            {
                Save = save;
            }
        }

        private void OnSaveSample()
        {
            if (Save != null)
            {
                m_eventAggregator.PublishOnBackgroundThread(new EvtSampleSaving() { Type = SaveType.SaveSamples, Save = Save });
            }
        }
    }
}

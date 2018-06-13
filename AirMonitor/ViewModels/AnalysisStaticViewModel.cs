using AirMonitor.Chart;
using AirMonitor.Config;
using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using AirMonitor.Map;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.ViewModels
{
    public class AnalysisStaticViewModel : Screen, IHandle<EvtChartScatterSelectChanged>
    {
        private IChartManager m_chartManager;
        private IEventAggregator m_eventAggregator;

        public AnalysisStaticViewModel(
            IChartManager chartManager,
            IConfigManager configManager,
            IEventAggregator eventAggregator)
        {
            m_chartManager = chartManager;
            m_eventAggregator = eventAggregator;
            m_eventAggregator.Subscribe(this);
            CorrectHeight = configManager.GetConfig<AirStandardSetting>().CorrectAltitude;
        }

        public override void TryClose(bool? dialogResult = null)
        {
            base.TryClose(dialogResult);
            m_eventAggregator.Unsubscribe(this);
        }


        public IMapView MapView { get; set; }

        public MapBlock[] MapBlocks { get; set; }

        public AnalysisMode Mode { get; set; }

        public MapPointData[] Points { get; private set; }
        /// <summary>
        /// 高度差
        /// </summary>
        public double HeightDifference { get; private set; }

        public double MaxHeight { get; private set; }

        public double MinHeight { get; private set; }

        public double CorrectHeight { get; set; }

        public object PlotModel { get; set; }

        public void OnMapBlocksChanged()
        {
            Points = MapBlocks?.SelectMany(o => o.points).ToArray();
            if (Points != null && Points.Any())
            {
                MaxHeight = Points.Max(o => o.hight);
                MinHeight = Points.Min(o => o.hight);
                HeightDifference = MaxHeight - MinHeight;
                RefreshPlot();
            }
        }

        private ObservableCollection<Tuple<double, double>> CreatePlotData()
        {
            return new ObservableCollection<Tuple<double, double>>(Points.Select(o =>
                Tuple.Create(Mode == AnalysisMode.Horizontal ? o.ActualLng : o.ActualLat, o.hight - CorrectHeight)
                ));
        }

        public void RefreshPlot()
        {
            var p = typeof(MapPointData).GetProperty(MapView.MapGridOptions.dataName);
            if (p != null)
            {
                PlotModel = m_chartManager.CreateScatter(
                    new ObservableCollection<ScatterData>(
                        Points.Select(o => new ScatterData()
                        {
                            X = Mode == AnalysisMode.Horizontal ? o.ActualLng : o.ActualLat,
                            Y = o.hight - CorrectHeight,
                            Value = (double)p.GetValue(o),
                            Tag = o
                        }))
                    , new ScatterOptions()
                    {
                        MaxVaule = MapView.MapGridOptions.maxValue,
                        MinValue = MapView.MapGridOptions.minValue,
                        MaxColor = MapView.MapGridOptions.colorEnd,
                        MinColor = MapView.MapGridOptions.colorBegin,
                    });
            }
        }

        public void Handle(EvtChartScatterSelectChanged message)
        {
            if (message.Scatter == PlotModel)
            {
                if (message.Data.Any())
                {
                    MapView.MapProvider.MapShowTempReport(message.Data.Select(o => o.Tag as MapPointData).ToArray());
                }
                else
                {
                    MapView.MapProvider.MapClearTempReport();
                }
            }
        }
    }
}

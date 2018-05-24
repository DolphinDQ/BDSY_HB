using AirMonitor.Chart;
using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using AirMonitor.Map;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.ViewModels
{
    /// <summary>
    /// 区域动态分析。
    /// </summary>
    public class DynamicAnalysisViewModel : Screen,
        IHandle<EvtMapPointConverted>,
        IHandle<EvtAirSample>
    {
        private IChartManager m_chartManager;
        private IEventAggregator m_eventAggregator;

        public DynamicAnalysisViewModel(
            IEventAggregator eventAggregator,
            IChartManager chartManager)
        {
            m_chartManager = chartManager;
            m_eventAggregator = eventAggregator;
            AnalysisModes = new[]
            {
                Tuple.Create(AnalysisMode.Horizontal,"横向"),
                Tuple.Create(AnalysisMode.Vertical, "纵向"),
            };
            m_eventAggregator.Subscribe(this);
        }


        public override void TryClose(bool? dialogResult = null)
        {
            base.TryClose(dialogResult);
            m_eventAggregator.Unsubscribe(this);
        }

        public IMapView MapView { get; set; }

        public Tuple<AnalysisMode, string>[] AnalysisModes { get; set; }

        public AnalysisMode Mode { get; set; } = AnalysisMode.Horizontal;

        public List<EvtAirSample> Points { get; } = new List<EvtAirSample>();
        /// <summary>
        /// 高度差
        /// </summary>
        public double HeightDifference { get; private set; }

        public double MaxHeight { get; private set; }

        public double MinHeight { get; private set; }

        private ObservableCollection<ScatterData> PlotData { get; } = new ObservableCollection<ScatterData>();

        public object PlotModel { get; set; }

        public double CorrectHeight { get; set; }

        public EvtMapSelectAnalysisArea Bounds { get; set; }
        public PropertyInfo DataProperty => typeof(MapPointData).GetProperty(MapView.MapGridOptions.dataName);

        public void Handle(EvtAirSample message)
        {
            if (Bounds != null && MapView.Sampling)
            {
                OnUIThread(() =>
                {
                    Points.Add(message);
                    NotifyOfPropertyChange(nameof(Points));
                    MaxHeight = Points.Max(o => o.hight);
                    MinHeight = Points.Min(o => o.hight);
                    HeightDifference = MaxHeight - MinHeight;
                });
            }
        }
        public void Handle(EvtMapPointConverted message)
        {
            OnUIThread(() =>
            {
                var p = Points.FirstOrDefault(o => o.GetHashCode() == message.Seq);
                if (p == null) return;
                PlotData.Add(CreateScatterData(p));
            });
        }

        public void OnBoundsChanged()
        {
            if (Bounds != null)
            {
                RefreshSample();
            }
        }

        private ScatterData CreateScatterData(EvtAirSample source)
        {
            return new ScatterData()
            {
                X = Mode == AnalysisMode.Horizontal ? source.ActualLng : source.ActualLat,
                Y = source.hight - CorrectHeight,
                Value = (double)DataProperty.GetValue(source),
                Tag = source
            };
        }

        public void RefreshSample()
        {
            PlotData.Clear();
            foreach (var item in Points)
            {
                PlotData.Add(CreateScatterData(item));
            }
            PlotModel = m_chartManager.CreateScatter(PlotData
                , new ScatterOptions()
                {
                    MaxX = Mode == AnalysisMode.Horizontal ? Bounds.ne.lng : Bounds.ne.lat,
                    MinX = Mode == AnalysisMode.Horizontal ? Bounds.sw.lng : Bounds.sw.lat,
                    MaxVaule = MapView.MapGridOptions.maxValue,
                    MinValue = MapView.MapGridOptions.minValue,
                    MaxColor = MapView.MapGridOptions.colorEnd,
                    MinColor = MapView.MapGridOptions.colorBegin,
                });
        }

        public void ClearSample()
        {
            Points.Clear();
            RefreshSample();
        }

    }
}

using AirMonitor.Chart;
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
    public class SampleAnalysisViewModel : Screen
    {
        private IChartManager m_chartManager;

        public enum AnalysisMode
        {
            Vertical,
            Horizontal,
        }

        public SampleAnalysisViewModel(IChartManager chartManager)
        {
            m_chartManager = chartManager;
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
    }
}

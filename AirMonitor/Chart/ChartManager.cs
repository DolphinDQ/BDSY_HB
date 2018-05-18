using AirMonitor.Interfaces;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.Concurrent;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Axes;
using System.Collections.ObjectModel;

namespace AirMonitor.Chart
{
    class ChartManager : IChartManager
    {
        private static TimeSpan Span { get; set; } = TimeSpan.FromSeconds(60);

        public object CreateLiner(ObservableCollection<Tuple<DateTime, double>> data, LinerOptions options = null)
        {
            var plot = new PlotModel { IsLegendVisible = false, Padding = new OxyThickness(2), PlotAreaBorderThickness = new OxyThickness(0) };
            plot.Axes.Add(new LinearAxis()
            {
                IsAxisVisible = false,
                Maximum = options?.MaxVaule ?? double.NaN,
                Minimum = options?.MinValue ?? double.NaN
            });
            plot.Axes.Add(new DateTimeAxis()
            {
                IsAxisVisible = false,
                Position = AxisPosition.Bottom,
                Minimum = DateTimeAxis.ToDouble(DateTime.Now.Add(-Span)),
                Maximum = DateTimeAxis.ToDouble(DateTime.Now),
            });
            var series = new LineSeries { MarkerType = MarkerType.None };
            series.Points.AddRange(data.Select(o => new DataPoint(DateTimeAxis.ToDouble(o.Item1), o.Item2)));
            plot.Series.Add(series);
            data.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewItems == null) return;
                        var newItems = e.NewItems.Cast<Tuple<DateTime, double>>();
                        series.Points.AddRange(newItems.Select(o => new DataPoint(DateTimeAxis.ToDouble(o.Item1), o.Item2)));
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        series.Points.Clear();
                        break;
                    default:
                        break;
                }
                var axis = plot.Axes.FirstOrDefault(o => o.Position == AxisPosition.Bottom);
                if (DateTimeAxis.ToDateTime(axis.Maximum) < DateTime.Now)
                {
                    axis.Maximum = DateTimeAxis.ToDouble(DateTime.Now);
                    axis.Minimum = DateTimeAxis.ToDouble(DateTime.Now.Add(-Span));
                }
                plot.InvalidatePlot(true);
            };
            return plot;
        }

        public object CreateScatter(ObservableCollection<ScatterData> data, ScatterOptions options = null)
        {
            var plot = new PlotModel { IsLegendVisible = false, Padding = new OxyThickness(5) };
            var series = new ScatterSeries { MarkerType = MarkerType.Circle };
            series.Points.AddRange(data.Select(o => new ScatterPoint(o.X, o.Y, o.Size, o.Value, o.Tag)));
            plot.Series.Add(series);
            plot.Axes.Add(new LinearColorAxis() { Position = AxisPosition.Right });
            SetScatter(plot, options);
            data.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewItems == null) return;
                        var newItems = e.NewItems.Cast<ScatterData>();
                        series.Points.AddRange(newItems.Select(o => new ScatterPoint(o.X, o.Y, o.Size, o.Value, o.Tag)));
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        series.Points.Clear();
                        break;
                    default:
                        break;
                }
                plot.InvalidatePlot(true);
            };
            return plot;
        }

        public void SetScatter(object scatter, ScatterOptions options)
        {
            if (scatter is PlotModel plot)
            {
                var axis = plot.Axes.First(o => o.Position == AxisPosition.Right) as LinearColorAxis;
                if (options.MaxVaule != null)
                {
                    axis.Maximum = options.MaxVaule.Value;
                }
                if (options.MinValue != null)
                {
                    axis.Minimum = options.MinValue.Value;
                }
                if (options.MaxColor != null && options.MinColor != null)
                {
                    axis.Palette = OxyPalette.Interpolate(300, OxyColor.Parse(options.MinColor), OxyColor.Parse(options.MaxColor));
                }
                //if (options.MaxColor != null)
                //{
                //    axis.HighColor = ;
                //}
                //if (options.MinColor != null)
                //{
                //    axis.LowColor = ;
                //}
            }
        }


    }
}

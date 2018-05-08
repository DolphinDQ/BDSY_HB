using AirMonitor.Interfaces;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.Concurrent;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Axes;
using System.Collections.ObjectModel;

namespace AirMonitor.Core
{
    class ChartManager : IChartManager
    {
        private static TimeSpan Span { get; set; } = TimeSpan.FromSeconds(60);

        public ConcurrentDictionary<int, PlotModel> LinnerPlot { get; set; } = new ConcurrentDictionary<int, PlotModel>();

        public object CreatLiner(ObservableCollection<Tuple<DateTime, double>> data)
        {
            data.CollectionChanged -= Data_CollectionChanged;
            data.CollectionChanged += Data_CollectionChanged;
            var plot = new PlotModel { IsLegendVisible = false, Padding = new OxyThickness(2), PlotAreaBorderThickness = new OxyThickness(0) };
            plot.Axes.Add(new LinearAxis() { IsAxisVisible = false });
            plot.Axes.Add(new DateTimeAxis()
            {
                IsAxisVisible = false,
                Position = AxisPosition.Bottom,
                Minimum = DateTimeAxis.ToDouble(DateTime.Now.Add(-Span)),
                Maximum = DateTimeAxis.ToDouble(DateTime.Now),
            });
            var series = new LineSeries {  MarkerType = MarkerType.None };
            series.Points.AddRange(data.Select(o => new DataPoint(DateTimeAxis.ToDouble(o.Item1), o.Item2)));
            plot.Series.Add(series);
            LinnerPlot.TryAdd(data.GetHashCode(), plot);
            this.Info("add plot :{0}", data.GetHashCode());
            return plot;
        }

        private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Info("receive plot:{0} changed event.{1}", sender.GetHashCode(), e.Action);
            if (LinnerPlot.TryGetValue(sender.GetHashCode(), out var plot))
            {
                var series = plot.Series.FirstOrDefault() as LineSeries;
                if (series != null)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            if (e.NewItems == null) return;
                            var newItems = e.NewItems.Cast<Tuple<DateTime, double>>();
                            series.Points.AddRange(newItems.Select(o => new DataPoint(DateTimeAxis.ToDouble(o.Item1), o.Item2)));
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            break;
                        case NotifyCollectionChangedAction.Move:
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
                }
            }
        }
    }
}

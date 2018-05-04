using AirMonitor.Interfaces;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Axes;

namespace AirMonitor.Core
{
    class ChartProvider : IChartProvider
    {
        public ConcurrentDictionary<int, PlotModel> LinnerPlot { get; set; } = new ConcurrentDictionary<int, PlotModel>();

        public object CreatLiner(IObservableCollection<Tuple<DateTime, double>> data)
        {
            data.CollectionChanged -= Data_CollectionChanged;
            data.CollectionChanged += Data_CollectionChanged;
            var plot = new PlotModel();
            plot.Axes.Add(new LinearAxis() { IsAxisVisible = false });
            plot.Axes.Add(new DateTimeAxis() { IsAxisVisible = false, Position = AxisPosition.Bottom, MajorStep = TimeSpan.FromSeconds(1).Ticks });
            var series = new LineSeries { Smooth = true, MarkerType = MarkerType.None };
            series.Points.AddRange(data.Select(o => new DataPoint(o.Item1.Ticks, o.Item2)));
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
                    var newItems = e.NewItems.Cast<Tuple<DateTime, double>>();
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            series.Points.AddRange(newItems.Select(o => new DataPoint(o.Item1.Ticks, o.Item2)));
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            series.Points.RemoveAll(o => newItems.Any(i => i.Item1.Ticks == o.X));
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            break;
                        case NotifyCollectionChangedAction.Move:
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}

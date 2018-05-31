using AirMonitor.Interfaces;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.Concurrent;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Axes;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using AirMonitor.EventArgs;
using System.Threading.Tasks;

namespace AirMonitor.Chart
{
    class ChartManager : IChartManager
    {
        private IEventAggregator m_eventAggregator;
        private ConcurrentDictionary<int, object> m_selectDelay = new ConcurrentDictionary<int, object>();
        public ChartManager(IEventAggregator eventAggregator)
        {
            m_eventAggregator = eventAggregator;
        }

        private static TimeSpan Span { get; set; } = TimeSpan.FromSeconds(60);

        public object CreateLiner(ObservableCollection<Tuple<DateTime, double>> data, LinerOptions options = null)
        {
            var plot = new PlotModel { IsLegendVisible = false, Padding = new OxyThickness(2), PlotAreaBorderThickness = new OxyThickness(0) };
            plot.Axes.Add(new LinearAxis()
            {
                IsAxisVisible = false,
                Maximum = options?.MaxY ?? double.NaN,
                Minimum = options?.MaxY ?? double.NaN
            });
            plot.Axes.Add(new DateTimeAxis()
            {
                IsAxisVisible = false,
                Position = AxisPosition.Bottom,
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
                        var axis = plot.Axes.FirstOrDefault(o => o.Position == AxisPosition.Bottom);
                        var max = newItems.Max(o => o.Item1);
                        axis.Maximum = DateTimeAxis.ToDouble(max);
                        axis.Minimum = DateTimeAxis.ToDouble(max - Span);
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

        public object CreateScatter(ObservableCollection<ScatterData> data, ScatterOptions options = null)
        {
            var plot = new PlotModel { IsLegendVisible = false, Padding = new OxyThickness(5) };
            var series = new ScatterSeries { MarkerType = MarkerType.Circle };
            series.Points.AddRange(data.Select(o => new ScatterPoint(o.X, o.Y, o.Size, o.Value, o.Tag)));
            plot.Series.Add(series);
            series.SelectionMode = SelectionMode.Multiple;
            series.Selectable = true;
            series.SelectionChanged += Series_SelectionChanged;
            plot.Axes.Add(new LinearColorAxis() { Position = AxisPosition.Right });
            plot.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom });
            SetScatter(plot, options);
            plot.Updated += Plot_Updated; ;
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

        private void Plot_Updated(object sender, System.EventArgs e)
        {
            var plot = sender as PlotModel;
            if (plot.PlotView != null)
            {
                plot.PlotView.ActualController.UnbindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Control);
                plot.PlotView.ActualController.UnbindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Shift);
                var command = new DelegatePlotCommand<OxyMouseDownEventArgs>((p, c, arg) => c.AddMouseManipulator(p, new ScatterSelectManipulator(p), arg));
                plot.PlotView.ActualController.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Control, command);
                plot.PlotView.ActualController.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Shift, command);
            }
            plot.Updated -= Plot_Updated;
        }



        private async void Series_SelectionChanged(object sender, System.EventArgs e)
        {
            var serise = sender as ScatterSeries;
            var key = serise.GetHashCode();
            if (m_selectDelay.TryGetValue(key, out var _)) return;
            m_selectDelay.TryAdd(key, serise);
            await Task.Delay(100);
            m_selectDelay.TryRemove(key, out var _);
            m_eventAggregator.PublishOnBackgroundThread(new EvtChartScatterSelectChanged()
            {
                Scatter = serise.PlotModel,
                Data = serise.GetSelectedItems().Select(o => serise.Points[o]).Select(o => new ScatterData()
                {
                    Size = o.Size,
                    Tag = o.Tag,
                    Value = o.Value,
                    Y = o.Y,
                    X = o.X
                }).ToArray()
            });
        }

        public void SetScatter(object scatter, ScatterOptions options)
        {
            if (scatter is PlotModel plot)
            {
                var right = plot.Axes.First(o => o.Position == AxisPosition.Right) as LinearColorAxis;
                right.Maximum = options.MaxVaule ?? double.NaN;
                right.Minimum = options.MinValue ?? double.NaN;
                if (options.MaxColor != null && options.MinColor != null)
                {
                    right.Palette = OxyPalette.Interpolate(300, OxyColor.Parse(options.MinColor), OxyColor.Parse(options.MaxColor));
                }

                var bottom = plot.Axes.First(o => o.Position == AxisPosition.Bottom) as LinearAxis;
                bottom.Maximum = options.MaxX ?? double.NaN;
                bottom.Minimum = options.MinX ?? double.NaN;
            }

        }


    }
}

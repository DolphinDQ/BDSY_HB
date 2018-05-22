using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chart
{
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using System.CodeDom.Compiler;
    using System.ComponentModel;

    /// <summary>
    /// Represents the view-model for the main window.
    /// </summary>
    public class MainViewModel
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            // Create the plot model
            var model = new PlotModel { IsLegendVisible = false };
            // Create two line series (markers are hidden by default)
            var series1 = new ScatterSeries() { MarkerType = MarkerType.Circle };
            //m_series1 = new LineSeries { Title = "Series 1", Smooth = true, MarkerType = MarkerType.None };
            //m_series1.Points.Add(new DataPoint(1, 18));
            //series1.Points.Add(new DataPoint(10, 14));
            //series1.Points.Add(new DataPoint(15, 50));
            //series1.Points.Add(new DataPoint(20, 12));
            //series1.Points.Add(new DataPoint(25, 65));
            //series1.Points.Add(new DataPoint(30, 8));
            //series1.Points.Add(new DataPoint(35, 18));
            //series1.Points.Add(new DataPoint(40, 15));
            //series1.Points.Add(new DataPoint(45, 5));
            series1.SelectionChanged += Series1_SelectionChanged;
            series1.Points.Add(new ScatterPoint(10, 14, 20, 1000));
            series1.Points.Add(new ScatterPoint(15, 50, 30, 500));
            series1.Points.Add(new ScatterPoint(20, 12));
            series1.Points.Add(new ScatterPoint(25, 65));
            series1.Points.Add(new ScatterPoint(30, 8));
            series1.Points.Add(new ScatterPoint(35, 18));
            series1.Points.Add(new ScatterPoint(40, 15));
            series1.Points.Add(new ScatterPoint(45, 5));
            series1.Selectable = true;
            series1.SelectionMode = SelectionMode.Multiple;
            // Add the series to the plot model
            model.Series.Add(series1);
            // Axes are created automatically if they are not defined
            //tmp.Axes.Add(new LinearAxis() { });
            //tmp.Axes.Add(new DateTimeAxis() { Position = AxisPosition.Bottom, Minimum = DateTimeAxis.ToDouble(DateTime.Now), Maximum = DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(6)), });
            model.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Palette = OxyPalettes.Jet(200)  });
            // Set the Model property, the INotifyPropertyChanged event will make the WPF Plot control update its content
            this.Model = model;

            //var i = 2;
            //Task.Factory.StartNew(() =>
            //{
            //    do
            //    {
            //        m_series1.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), new Random().Next() % 10));
            //        Model.InvalidatePlot(true);
            //        Task.Delay(1000).Wait();
            //    } while (true);

            //});


        }

        private void Series1_SelectionChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Gets the plot model.
        /// </summary>
        public PlotModel Model { get; private set; }

    }
}

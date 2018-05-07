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
        private LineSeries m_series1;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            // Create the plot model
            var tmp = new PlotModel { IsLegendVisible = false, Padding = new OxyThickness(0), PlotAreaBorderThickness = new OxyThickness(0) };
            // Create two line series (markers are hidden by default)

            m_series1 = new LineSeries { Title = "Series 1", Smooth = true, MarkerType = MarkerType.None };
            //m_series1.Points.Add(new DataPoint(0, 0));
            //m_series1.Points.Add(new DataPoint(1, 18));
            //series1.Points.Add(new DataPoint(10, 14));
            //series1.Points.Add(new DataPoint(15, 50));
            //series1.Points.Add(new DataPoint(20, 12));
            //series1.Points.Add(new DataPoint(25, 65));
            //series1.Points.Add(new DataPoint(30, 8));
            //series1.Points.Add(new DataPoint(35, 18));
            //series1.Points.Add(new DataPoint(40, 15));
            //series1.Points.Add(new DataPoint(45, 5));

            // Add the series to the plot model
            tmp.Series.Add(m_series1);

            // Axes are created automatically if they are not defined
            tmp.Axes.Add(new LinearAxis() { });
            tmp.Axes.Add(new DateTimeAxis() {  Position = AxisPosition.Bottom,Minimum=DateTimeAxis.ToDouble(DateTime.Now), Maximum= DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(6)), });
            // Set the Model property, the INotifyPropertyChanged event will make the WPF Plot control update its content
            this.Model = tmp;
            var i = 2;
            Task.Factory.StartNew(() =>
            {
                do
                {
                    m_series1.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), new Random().Next() % 10));
                    Model.InvalidatePlot(true);
                    Task.Delay(1000).Wait();
                } while (true);

            });
        }

        /// <summary>
        /// Gets the plot model.
        /// </summary>
        public PlotModel Model { get; private set; }

    }
}

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
            var tmp = new PlotModel { IsLegendVisible = false ,Padding=new OxyThickness(0),PlotAreaBorderThickness=new OxyThickness(0) };
            // Create two line series (markers are hidden by default)
            var series1 = new LineSeries { Title = "Series 1", Smooth = true, MarkerType = MarkerType.None};
            series1.Points.Add(new DataPoint(0, 0));
            series1.Points.Add(new DataPoint(5, 18));
            series1.Points.Add(new DataPoint(10, 14));
            series1.Points.Add(new DataPoint(15, 50));
            series1.Points.Add(new DataPoint(20, 12));
            series1.Points.Add(new DataPoint(25, 65));
            series1.Points.Add(new DataPoint(30, 8));
            series1.Points.Add(new DataPoint(35, 18));
            series1.Points.Add(new DataPoint(40, 15));
            series1.Points.Add(new DataPoint(45, 5));

            // Add the series to the plot model
            tmp.Series.Add(series1);

            // Axes are created automatically if they are not defined
            tmp.Axes.Add(new LinearAxis() { IsAxisVisible = false });
            tmp.Axes.Add(new TimeSpanAxis() { IsAxisVisible = false, Position = AxisPosition.Bottom });
            // Set the Model property, the INotifyPropertyChanged event will make the WPF Plot control update its content
            this.Model = tmp;
        }

        /// <summary>
        /// Gets the plot model.
        /// </summary>
        public PlotModel Model { get; private set; }
    }
}

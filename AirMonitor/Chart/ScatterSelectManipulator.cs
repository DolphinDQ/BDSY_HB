using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Chart
{
    public class ScatterSelectManipulator : MouseManipulator
    {
        private OxyRect ZoomRectangle { get; set; }

        public ScatterSelectManipulator(IPlotView plotView) : base(plotView)
        {
        }
        public override void Completed(OxyMouseEventArgs e)
        {
            base.Completed(e);
            PlotView.HideZoomRectangle();
            var p0 = InverseTransform(ZoomRectangle.Left, ZoomRectangle.Bottom);
            var p1 = InverseTransform(ZoomRectangle.Right, ZoomRectangle.Top);
            var serise = PlotView.ActualModel.Series.FirstOrDefault(o => o is ScatterSeries) as ScatterSeries;
            if (serise != null)
            {
                for (int i = 0; i < serise.ActualPoints.Count; i++)
                {
                    var point = serise.ActualPoints[i];
                    if (point.X > p0.X && point.X < p1.X && point.Y > p0.Y && point.Y < p1.Y)
                    {
                        if (e.IsShiftDown)
                        {
                            if (serise.IsItemSelected(i))
                            {
                                serise.UnselectItem(i);
                            }
                        }
                        else
                        {
                            serise.SelectItem(i);
                        }
                    }
                }
                PlotView.InvalidatePlot();
            }
        }
        public override void Delta(OxyMouseEventArgs e)
        {
            base.Delta(e);

            var plotArea = PlotView.ActualModel.PlotArea;

            var x = Math.Min(StartPosition.X, e.Position.X);
            var w = Math.Abs(StartPosition.X - e.Position.X);
            var y = Math.Min(StartPosition.Y, e.Position.Y);
            var h = Math.Abs(StartPosition.Y - e.Position.Y);

            if (XAxis == null || !XAxis.IsZoomEnabled)
            {
                x = plotArea.Left;
                w = plotArea.Width;
            }

            if (YAxis == null || !YAxis.IsZoomEnabled)
            {
                y = plotArea.Top;
                h = plotArea.Height;
            }

            ZoomRectangle = new OxyRect(x, y, w, h);
            PlotView.ShowZoomRectangle(ZoomRectangle);
            e.Handled = true;
        }
        public override void Started(OxyMouseEventArgs e)
        {
            base.Started(e);
            ZoomRectangle = new OxyRect(StartPosition.X, StartPosition.Y, 0, 0);
            PlotView.ShowZoomRectangle(ZoomRectangle);
            e.Handled = true;
        }
    }
}

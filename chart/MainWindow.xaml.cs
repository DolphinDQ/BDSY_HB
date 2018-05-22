using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace chart
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private class SelectManipulator : MouseManipulator
        {
            /// <summary>
            /// The zoom rectangle.
            /// </summary>
            private OxyRect zoomRectangle;

            public SelectManipulator(IPlotView plotView) : base(plotView)
            {
            }

            public override void Completed(OxyMouseEventArgs e)
            {
                base.Completed(e);
                this.PlotView.HideZoomRectangle();
                //if (this.zoomRectangle.Width > 10 && this.zoomRectangle.Height > 10)
                //{
                //    var p0 = this.InverseTransform(this.zoomRectangle.Left, this.zoomRectangle.Top);
                //    var p1 = this.InverseTransform(this.zoomRectangle.Right, this.zoomRectangle.Bottom);

                //    if (this.XAxis != null)
                //    {
                //        this.XAxis.Zoom(p0.X, p1.X);
                //    }

                //    if (this.YAxis != null)
                //    {
                //        this.YAxis.Zoom(p0.Y, p1.Y);
                //    }

                //    this.PlotView.InvalidatePlot();
                //}

                var p0 = this.InverseTransform(this.zoomRectangle.Left, this.zoomRectangle.Top);
                var p1 = this.InverseTransform(this.zoomRectangle.Right, this.zoomRectangle.Bottom);
                var s = PlotView.ActualModel.Series;


                e.Handled = true;
            }
            public override void Delta(OxyMouseEventArgs e)
            {
                base.Delta(e);
                var plotArea = this.PlotView.ActualModel.PlotArea;
                var x = Math.Min(this.StartPosition.X, e.Position.X);
                var w = Math.Abs(this.StartPosition.X - e.Position.X);
                var y = Math.Min(this.StartPosition.Y, e.Position.Y);
                var h = Math.Abs(this.StartPosition.Y - e.Position.Y);
                x = plotArea.Left;
                w = plotArea.Width;
                y = plotArea.Top;
                h = plotArea.Height;
                this.zoomRectangle = new OxyRect(x, y, w, h);
                this.PlotView.ShowZoomRectangle(this.zoomRectangle);
                e.Handled = true;
            }
            public override void Started(OxyMouseEventArgs e)
            {
                base.Started(e);
                this.zoomRectangle = new OxyRect(this.StartPosition.X, this.StartPosition.Y, 0, 0);
                this.PlotView.ShowZoomRectangle(this.zoomRectangle);
                e.Handled = true;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            Plot.ActualController.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Control, new DelegatePlotCommand<OxyMouseDownEventArgs>(OnClick));
        }

        private void OnClick(IPlotView arg1, IController arg2, OxyMouseDownEventArgs arg3)
        {
            arg2.AddMouseManipulator(arg1, new SelectManipulator(arg1), arg3);
            //if (arg3.HitTestResult != null)
            //{
            //    var ser = arg1.ActualModel.Series.First();
            //    ser.SelectItem((int)arg3.HitTestResult.Index);
            //    arg1.ActualModel.InvalidatePlot(true);
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}

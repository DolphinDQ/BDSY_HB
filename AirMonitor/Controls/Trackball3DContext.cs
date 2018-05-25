using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace AirMonitor.Controls
{
    public class Trackball3DContext
    {
        public ScaleTransform3D Scale { get; set; }
        public RotateTransform3D Rotate { get; set; }
        public TranslateTransform3D Translate { get; set; }
        public TranslateTransform3D MouseDownTranslate { get; set; }
        public RotateTransform3D MouseDownRotate { get; set; }
        public Point MouseDownPoint { get; set; }
    }
}

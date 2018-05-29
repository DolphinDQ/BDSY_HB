using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Controls
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class Map3DPoint
    {

        public double Lat { get; set; }

        public double Lng { get; set; }

        public double Height { get; set; }
    }
}

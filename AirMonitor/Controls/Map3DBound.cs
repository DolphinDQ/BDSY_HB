using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Controls
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class Map3DBound
    {
        public Map3DPoint Max { get; set; } = new Map3DPoint();

        public Map3DPoint Min { get; set; } = new Map3DPoint();
    }
}

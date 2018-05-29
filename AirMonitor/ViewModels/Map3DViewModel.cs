using AirMonitor.Controls;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.ViewModels
{
    class Map3DViewModel : Screen
    {

        public object MapContainer { get; set; }

        public Map3DBound MapBound { get; set; } = new Map3DBound()
        {
            Max = new Map3DPoint()
            {
                Lat = 23,
                Lng = 112,
                Height = 155,
            },
            Min= new Map3DPoint()
            {
                Lat = 24,
                Lng = 120,
                Height = 100,
            }
        };
    }
}

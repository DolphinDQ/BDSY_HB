using AirMonitor.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.EventArgs
{
    public class EvtCameraGetDevices
    {
        public IEnumerable<CameraDevice> Devices { get; set; }
    }
}

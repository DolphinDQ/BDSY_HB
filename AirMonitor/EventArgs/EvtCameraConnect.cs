using AirMonitor.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.EventArgs
{
    public class EvtCameraConnect
    {
        public bool IsConnected { get; set; }

        public string Message { get; set; }
    }
}

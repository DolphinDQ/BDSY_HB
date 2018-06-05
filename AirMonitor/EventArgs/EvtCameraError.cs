using AirMonitor.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.EventArgs
{
    public class EvtCameraError
    {
        public CameraError Error { get; set; }

        public string ErrorMessage { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Config
{
    public class CameraSetting
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int ConnectionTimeout { get; set; }
        public int Volumes { get; set; }
        public string ChannelName { get; set; }
        public int ChannelIndex { get; set; }
        public string CameraId { get; set; }

    }
}

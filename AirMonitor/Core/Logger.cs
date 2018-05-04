using AirMonitor.Interfaces;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Core
{
    public class Logger : ILog
    {

        public void Error(Exception exception)
        {
            Debug.Print("[Error]" + exception.ToString());
        }

        public void Info(string format, params object[] args)
        {
            Debug.Print("[Info]" + format, args);
        }

        public void Warn(string format, params object[] args)
        {
            Debug.Print("[Warn]" + format, args);
        }

    }


}

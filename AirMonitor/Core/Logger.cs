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
        [Flags]
        public enum LogType
        {
            None = 0,
            Info = 1,
            Warn = 2,
            Error = 4,
        }


        public void Error(Exception exception)
        {
            Trace.Fail($"[{LogType.Error}]{exception}");
            Trace.TraceError(exception.Message, exception.ToString());
        }

        public void Info(string format, params object[] args)
        {
            Trace.TraceInformation($"[{LogType.Info}]{format}", args);
        }

        public void Warn(string format, params object[] args)
        {
            Trace.TraceWarning($"[{LogType.Warn}]{format}", args);
        }

    }


}

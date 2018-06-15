using Caliburn.Micro;
using System;

namespace AirMonitor
{
    public static class Global
    {
        public static void Error(this object obj, Exception exception) => LogManager.GetLog(typeof(ILog)).Error(exception);

        public static void Info(this object obj, string format, params object[] args) => LogManager.GetLog(typeof(ILog)).Info($"[{obj.GetType().Name}]" + format, args);

        public static void Warn(this object obj, string format, params object[] args) => LogManager.GetLog(typeof(ILog)).Warn($"[{obj.GetType().Name}]" + format, args);

    }
}

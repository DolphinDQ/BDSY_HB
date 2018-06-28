using AirMonitor.Map;
using Caliburn.Micro;
using System;
using System.Windows.Media;
using System.Linq;
using AirMonitor.Config;

namespace AirMonitor
{
    public static class Global
    {
        public static void Error(this object obj, Exception exception) => LogManager.GetLog(typeof(ILog)).Error(exception);

        public static void Info(this object obj, string format, params object[] args) => LogManager.GetLog(typeof(ILog)).Info($"[{obj.GetType().Name}]" + format, args);

        public static void Warn(this object obj, string format, params object[] args) => LogManager.GetLog(typeof(ILog)).Warn($"[{obj.GetType().Name}]" + format, args);

        public static string GetColor(this MapGridOptions options, double value) => options.pollutant.GetColor(value);

        public static string GetColor(this AirPollutant pollutant, double value)
        {
            if (pollutant.Levels != null)
            {
                var minValue = pollutant.MinValue;
                var maxValue = pollutant.MaxValue;
                if (value > maxValue) value = maxValue;
                if (value < minValue) value = minValue;
                var level = pollutant.Levels.FirstOrDefault(o => o.MaxValue >= value && o.MinValue <= value);
                maxValue = level.MaxValue;
                minValue = level.MinValue;
                var percent = (value - minValue) / (maxValue - minValue);
                var begin = (Color)ColorConverter.ConvertFromString(level.MinColor);
                var end = (Color)ColorConverter.ConvertFromString(level.MaxColor);
                var r = GetColorValue(percent, begin.R, end.R);
                var g = GetColorValue(percent, begin.G, end.G);
                var b = GetColorValue(percent, begin.B, end.B);
                return "#" + r + g + b;
            }
            return "#ffffff";
        }

        private static string GetColorValue(double percent, double begin, double end)
        {
            return ((int)((end - begin) * percent + begin)).ToString("x2");
        }
    }
}

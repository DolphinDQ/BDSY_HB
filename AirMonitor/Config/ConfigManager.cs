using AirMonitor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AirMonitor.EventArgs;

namespace AirMonitor.Config
{
    public class ConfigManager : IConfigManager
    {
        private string Dir
        {
            get
            {
                var dir = AppDomain.CurrentDomain.BaseDirectory + "Config\\";
                if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }
                return dir;
            }
        }

        public T GetConfig<T>()
            where T : new()
        {
            var file = typeof(T).Name;
            T result;
            if (!File.Exists(Dir + file))
            {
                result = CreateDefault<T>();
                SaveConfig(result);
            }
            else
            {
                result = JsonConvert.DeserializeObject<T>(File.ReadAllText(Dir + file));
            }
            return result;
        }

        private T CreateDefault<T>()
            where T : new()
        {
            var type = typeof(T).Name;
            switch (type)
            {
                case nameof(AirStandardSetting):
                    return (T)CreateAirStandardSetting();
                default:
                    return new T();
            }

        }


        private object CreateAirStandardSetting()
        {
            return new AirStandardSetting()
            {
                Pollutant = new[] {
                    new AirPollutant(){ Name=nameof( EvtAirSample.temp), MinValue = 0 , MaxValue = 60 },
                    new AirPollutant(){ Name=nameof( EvtAirSample.humi), MinValue = 0 , MaxValue = 100 },
                    new AirPollutant(){ Name=nameof( EvtAirSample.voc), MinValue = 0 , MaxValue = 250 },
                    new AirPollutant(){ Name=nameof( EvtAirSample.co), MinValue = 0 , MaxValue = 4 },
                    new AirPollutant(){ Name=nameof( EvtAirSample.so2), MinValue = 0 , MaxValue = 300 },
                    new AirPollutant(){ Name=nameof( EvtAirSample.no2), MinValue = 0 , MaxValue = 500 },
                    new AirPollutant(){ Name=nameof( EvtAirSample.o3), MinValue = 0 , MaxValue = 300 },
                    new AirPollutant(){ Name=nameof( EvtAirSample.pm25), MinValue = 0 , MaxValue = 200 },
                    new AirPollutant(){ Name=nameof( EvtAirSample.pm10), MinValue = 0 , MaxValue = 250 },
                 },
            };
        }

        public void SaveConfig<T>(T config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            File.WriteAllText(Dir + typeof(T).Name, JsonConvert.SerializeObject(config));
        }
    }
}

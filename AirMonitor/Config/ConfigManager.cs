using AirMonitor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AirMonitor.EventArgs;
using Caliburn.Micro;

namespace AirMonitor.Config
{
    public class ConfigManager : IConfigManager
    {
        private IResourceManager m_res;
        private IEventAggregator m_eventAggregator;

        public ConfigManager(IResourceManager res,
            IEventAggregator eventAggregator)
        {
            m_res = res;
            m_eventAggregator = eventAggregator;
        }

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
                case nameof(CameraSetting):
                    return (T)CreateCameraSetting();
                case nameof(FtpSetting):
                    return (T)CreateFtpSetting();
                case nameof(FtpWriteSetting):
                    return (T)CreateFtpWriteSetting();
                default:
                    return new T();
            }

        }

        private object CreateFtpSetting()
        {
            return new FtpSetting()
            {
                Account = "airhunter",
                Password = "123456",
                Host = "b.vvlogic.com",
                Port = 8021,
                Root = "airhunter",
                SharedRoot = "shared",
            };
        }

        private object CreateFtpWriteSetting()
        {
            return new FtpWriteSetting()
            {
                Account = "airhunter",
                Password = "123456",
                Host = "192.168.1.180",
                Port = 21,
                Root = "airhunter",
                SharedRoot = "shared"
            };
        }

        private object CreateCameraSetting()
        {
            return new CameraSetting()
            {
                Host = "47.92.130.204",
                Port = 9701,
                UserName = "20180208",
                Password = "123456",
                ConnectionTimeout = 30,
                Volumes = 80,
                CameraId = null,
                ChannelIndex = 0,
            };
        }

        private object CreateAirStandardSetting()
        {
            return new AirStandardSetting()
            {
                CorrectAltitude = 100,
                MaxAltitude = 200,
                AltitudeUnit = "m",
                Pollutant = new[] {
                    new AirPollutant(){ Name=nameof( EvtAirSample.temp), MinValue = 0 , MaxValue = 100, DisplayName= m_res.GetText("T_Temperature") , Unit="℃"},
                    new AirPollutant(){ Name=nameof( EvtAirSample.humi), MinValue = 0 , MaxValue = 100, DisplayName= m_res.GetText("T_Humidity") , Unit="%" },
                    new AirPollutant(){ Name=nameof( EvtAirSample.voc), MinValue = 0 , MaxValue = 250 , DisplayName= m_res.GetText("T_VOC") , Unit= "ppb" },
                    new AirPollutant(){ Name=nameof( EvtAirSample.co), MinValue = 0 , MaxValue = 4, DisplayName= m_res.GetText("T_CO") , Unit= "mg/m3"  },
                    new AirPollutant(){ Name=nameof( EvtAirSample.so2), MinValue = 0 , MaxValue = 300, DisplayName= m_res.GetText("T_SO2") , Unit= "ug/m3"  },
                    new AirPollutant(){ Name=nameof( EvtAirSample.no2), MinValue = 0 , MaxValue = 500, DisplayName= m_res.GetText("T_NO2") , Unit= "ug/m3"  },
                    new AirPollutant(){ Name=nameof( EvtAirSample.o3), MinValue = 0 , MaxValue = 300, DisplayName= m_res.GetText("T_O3") , Unit= "ug/m3"  },
                    new AirPollutant(){ Name=nameof( EvtAirSample.pm25), MinValue = 0 , MaxValue = 200, DisplayName= m_res.GetText("T_PM2_5") , Unit= "ug/m3" },
                    new AirPollutant(){ Name=nameof( EvtAirSample.pm10), MinValue = 0 , MaxValue = 250, DisplayName= m_res.GetText("T_PM10") , Unit="ug/m3"  },
                 },
            };
        }

        public void SaveConfig<T>(T config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            File.WriteAllText(Dir + typeof(T).Name, JsonConvert.SerializeObject(config));
            m_eventAggregator.PublishOnBackgroundThread(new EvtSetting() { Command = SettingCommands.Changed, SettingObject = config });
        }
    }
}

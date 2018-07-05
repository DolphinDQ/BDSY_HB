﻿using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                default:
                    return new T();
            }

        }

        private object CreateFtpSetting()
        {
            return new FtpSetting()
            {
                Host = "b.vvlogic.com",
                Port = 8021,
                Account = "ftpguest",
            };
        }

        private object CreateCameraSetting()
        {
            return new CameraSetting();
        }

        private object CreateAirStandardSetting()
        {
            return new AirStandardSetting()
            {
                CorrectAltitude = 160,
                MaxAltitude = 200,
                AltitudeUnit = "m",
                Pollutant = new[] {
                    new AirPollutant(){ Name=nameof( EvtAirSample.temp),DisplayName= m_res.GetText("T_Temperature") , Unit="℃" ,Levels=CreateLevel(new []{0d,27,30,37,45,70},          new [] {"#00ff00","#66ff00","#ccff00","#ffca00","#ff6a00","#ff0000"})},
                    new AirPollutant(){ Name=nameof( EvtAirSample.humi),DisplayName= m_res.GetText("T_Humidity") , Unit="%"     ,Levels=CreateLevel(new []{0d,20,40,60,80,100},         new [] {"#00ff00","#66ff00","#ccff00","#ffca00","#ff6a00","#ff0000"})},
                    new AirPollutant(){ Name=nameof( EvtAirSample.voc), DisplayName= m_res.GetText("T_VOC") , Unit= "ppb"       ,Levels=CreateLevel(new []{0d,200,400,600,800,1000},    new [] {"#00ff00","#66ff00","#ccff00","#ffca00","#ff6a00","#ff0000"})},
                    new AirPollutant(){ Name=nameof( EvtAirSample.co),  DisplayName= m_res.GetText("T_CO") , Unit= "mg/m3"      ,Levels=CreateLevel(new []{0d,4.8,8,16,24,36},          new [] {"#00ff00","#66ff00","#ccff00","#ffca00","#ff6a00","#ff0000"})},
                    new AirPollutant(){ Name=nameof( EvtAirSample.so2), DisplayName= m_res.GetText("T_SO2") , Unit= "ug/m3"     ,Levels=CreateLevel(new []{0d,72,150,200,300,500},      new [] {"#00ff00","#66ff00","#ccff00","#ffca00","#ff6a00","#ff0000"})},
                    new AirPollutant(){ Name=nameof( EvtAirSample.no2), DisplayName= m_res.GetText("T_NO2") , Unit= "ug/m3"     ,Levels=CreateLevel(new []{0d,60,100,150,250,400},      new [] {"#00ff00","#66ff00","#ccff00","#ffca00","#ff6a00","#ff0000"})},
                    new AirPollutant(){ Name=nameof( EvtAirSample.o3),  DisplayName= m_res.GetText("T_O3") , Unit= "ug/m3"      ,Levels=CreateLevel(new []{0d,192,300,400,500,700},     new [] {"#00ff00","#66ff00","#ccff00","#ffca00","#ff6a00","#ff0000"})},
                    new AirPollutant(){ Name=nameof( EvtAirSample.pm25),DisplayName= m_res.GetText("T_PM2_5") , Unit= "ug/m3"   ,Levels=CreateLevel(new []{0d,44,80,150,200,300},       new [] {"#00ff00","#66ff00","#ccff00","#ffca00","#ff6a00","#ff0000"})},
                    new AirPollutant(){ Name=nameof( EvtAirSample.pm10),DisplayName= m_res.GetText("T_PM10") , Unit="ug/m3"     ,Levels=CreateLevel(new []{0d ,71,150,200,300,500},     new [] { "#00ff00", "#66ff00", "#ccff00", "#ffca00", "#ff6a00", "#ff0000" } )}
            },
            };
        }

        private AirPollutantLevel[] CreateLevel(double[] val, string[] color)
        {
            var list = new List<AirPollutantLevel>();
            for (int i = 0; i < val.Length - 1; i++)
            {
                list.Add(new AirPollutantLevel()
                {
                    Name = "Lv." + (i + 1),
                    MinValue = val[i],
                    MaxValue = val[i + 1],
                    MinColor = color[i],
                    MaxColor = color[i + 1]
                });
            }
            return list.ToArray();
        }

        public void SaveConfig<T>(T config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            File.WriteAllText(Dir + typeof(T).Name, JsonConvert.SerializeObject(config));
            m_eventAggregator.PublishOnBackgroundThread(new EvtSetting() { Command = SettingCommands.Changed, SettingObject = config });
        }
    }
}

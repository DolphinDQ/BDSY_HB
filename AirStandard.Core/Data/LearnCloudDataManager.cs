using AirStandard.Core.Interfaces;
using AirStandard.Model;
using LeanCloud;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AirStandard.Core.Data
{
    public class LearnCloudDataManager : IDataManager
    {
        private LearnCloudOptions m_config;
        private Timer m_timer;

        public LearnCloudDataManager(IOptions<LearnCloudOptions> options)
        {
            m_config = options.Value;
        }

        public void Dispose()
        {
            m_timer?.Dispose();
        }


        public void Init()
        {
            Console.WriteLine("appid {0}", m_config.AppId);
            try
            {
                AVClient.Initialize(m_config.AppId, m_config.AppKey);
                if (!Directory.Exists(m_config.DataDir))
                {
                    Directory.CreateDirectory(m_config.DataDir);
                }
                m_timer = new Timer(OnQueryOneHourData, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
           
        }

        /// <summary>
        /// 定期查询上个小时的数据。
        /// </summary>
        /// <param name="state"></param>
        private async void OnQueryOneHourData(object state)
        {
            await GetSample(DateTime.Now.AddHours(-1));
        }

        public async Task<IEnumerable<StandardSample>> GetSample(DateTime date)
        {
            if (date > CurrentHour(DateTime.Now))
            {
                return Enumerable.Empty<StandardSample>();
            }
            var dir = m_config.DataDir + date.ToString("yyyy-MM-dd/");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var file = dir + date.Hour;
            StandardSample[] data;
            if (File.Exists(file))
            {
                data = JsonConvert.DeserializeObject<StandardSample[]>(File.ReadAllText(file));
            }
            else
            {
                var query = new AVQuery<AVObject>("aqia");
                var obj = await query.Where(o => o.CreatedAt < CurrentHour(date).AddHours(1) && o.CreatedAt > CurrentHour(date).AddHours(0)).FirstOrDefaultAsync();
                data = new[] {
                    CreateSample(obj,"st0","天气预报"),
                    CreateSample(obj,"st1","湾梁"),
                    CreateSample(obj,"st2","华材职中"),
                    CreateSample(obj,"st3","南海气象局"),
                    CreateSample(obj,"st4","顺德苏岗"),
                    CreateSample(obj,"st5","容桂街道办"),
                    CreateSample(obj,"st6","高明孔堂"),
                    CreateSample(obj,"st7","三水云东海"),
                };
                File.WriteAllText(file, JsonConvert.SerializeObject(data));
            }
            return data;
        }



        private StandardSample CreateSample(AVObject obj, string key, string name)
        {
            return new StandardSample()
            {
                Station = name,
                co = GetValue<double>(obj, key + "co"),
                humi = double.TryParse(GetValue<string>(obj, key + "humi")?.Replace("%", ""), out var h) ? h / 100 : 0,
                no2 = GetValue<double>(obj, key + "no2"),
                o3 = GetValue<double>(obj, key + "o3"),
                pm10 = GetValue<double>(obj, key + "pm10"),
                pm25 = GetValue<double>(obj, key + "pm25"),
                so2 = GetValue<double>(obj, key + "so2"),
                temp = GetValue<double>(obj, key + "temp"),
                time = GetValue<string>(obj, key + "time"),
                voc = GetValue<double>(obj, key + "voc"),
            };
        }

        public T GetValue<T>(AVObject obj, string key)
        {
            try
            {
                if (obj.TryGetValue<T>(key, out var t))
                {
                    return t;
                }
            }
            catch (Exception)
            {

            }
            return default(T);
        }

        private DateTime CurrentHour(DateTime date) => date.Date.AddHours(date.Hour);

    }
}

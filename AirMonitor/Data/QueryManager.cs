using AirMonitor.Config;
using AirMonitor.Interfaces;
using AirStandard.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Data
{
    class QueryManager : IDataQueryManager
    {
        private HttpClient m_client;
        private AirStandardSetting m_config;

        public QueryManager(IConfigManager config)
        {

            m_config = config.GetConfig<AirStandardSetting>();
        }

        public void Dispose()
        {

        }

        public async Task<IEnumerable<StandardSample>> GetSamples(DateTime time)
        {
            if (m_client != null)
            {
                var resp = await m_client.GetAsync("standardsample/getsamples?time=" + time.ToUniversalTime().ToString());
                if (resp.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<IEnumerable<StandardSample>>(await resp.Content.ReadAsStringAsync());
                }
            }
            return Enumerable.Empty<StandardSample>();
        }

        public void Init()
        {
            m_client = new HttpClient();
            m_client.BaseAddress = new Uri(m_config.Server + "api/");
        }



    }
}

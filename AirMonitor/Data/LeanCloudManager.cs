using AirMonitor.Interfaces;
using LeanCloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Data
{
    class LeanCloudManager : IDataQueryManager
    {

        //private const string APPID = "sW1vwRHB97lVuLI0s9cusrAs-gzGzoHsz";
        //private const string APPKEY = "490sggiQarrolqNqidWb4i1y";

        public void Dispose()
        {

        }

        public void Init()
        {
            //AVClient.Initialize(APPID, APPKEY);
            //AVQuery<AVObject> query = new AVQuery<AVObject>("aqia");
            //var obj = await query.Where(o => o.CreatedAt > DateTime.Now - TimeSpan.FromDays(5)).FindAsync();
        }



    }
}

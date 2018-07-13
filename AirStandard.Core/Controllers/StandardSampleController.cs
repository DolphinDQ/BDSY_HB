using AirStandard.Core.Interfaces;
using AirStandard.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirStandard.Core.Controllers
{
    public class StandardSampleController : Controller
    {
        private IDataManager m_data;

        public StandardSampleController(IDataManager data)
        {
            m_data = data;
        }


        [HttpGet]
        public async Task<IEnumerable<StandardSample>> GetSamples(DateTime time)
        {
            return await m_data.GetSample(time);
        }

        [HttpGet]
        public string Get()
        {
            return "123";
        }
    }
}

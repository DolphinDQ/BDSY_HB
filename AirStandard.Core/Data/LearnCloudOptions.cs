using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirStandard.Core.Data
{
    public class LearnCloudOptions
    {
        public string AppId { get; set; }

        public string AppKey { get; set; }

        public string DataDir { get; set; } = "./Data/";
    }
}

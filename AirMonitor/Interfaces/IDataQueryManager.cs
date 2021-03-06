﻿using AirStandard.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Interfaces
{
    public interface IDataQueryManager : IDisposable
    {
        void Init();

        Task<IEnumerable<StandardSample>> GetSamples(DateTime time);
    }
}

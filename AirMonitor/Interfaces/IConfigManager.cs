using AirMonitor.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Interfaces
{
    public interface IConfigManager
    {
        T GetConfig<T>() where T : new();

        void SaveConfig<T>(T config);
    }

}

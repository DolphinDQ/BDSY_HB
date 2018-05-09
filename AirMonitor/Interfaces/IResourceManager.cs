using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Interfaces
{
    public interface IResourceManager
    {
        string GetText(string key);

        object Resource(string key);
    }
}

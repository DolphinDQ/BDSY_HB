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

        string GetText(Enum key);

        object Resource(string key);
    }
}

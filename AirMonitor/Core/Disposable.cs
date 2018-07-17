using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Core
{
    public class Disposable : IDisposable
    {
        private Action m_dispose;

        public Disposable(Action dispose)
        {
            m_dispose = dispose;
        }

        public void Dispose() => m_dispose?.Invoke();
    }
}

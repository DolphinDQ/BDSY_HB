using AirMonitor.EventArgs;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.ViewModels
{
    class SaveSampleViewModel : Screen
    {
        public SaveSampleViewModel()
        {
        }

        public EvtSampleSaving Evt { get; internal set; }
    }
}

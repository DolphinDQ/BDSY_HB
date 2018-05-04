using AirMonitor.Data;
using AirMonitor.Interfaces;
using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AirMonitor.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class DataDisplayViewModel : Screen, IHandle<EnvironmentCallback>
    {
        private IEventAggregator m_eventAggregator;

        public EnvironmentCallback NewestData { get; set; }


        public DataDisplayViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);
            m_eventAggregator = eventAggregator;
        }

        public override void TryClose(bool? dialogResult = null)
        {
            base.TryClose(dialogResult);
            m_eventAggregator.Unsubscribe(this);
        }

    
        public void Handle(EnvironmentCallback message)
        {
            NewestData = message;
        }
    }
}

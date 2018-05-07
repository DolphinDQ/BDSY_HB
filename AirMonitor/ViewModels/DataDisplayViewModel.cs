using AirMonitor.Data;
using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AirMonitor.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class DataDisplayViewModel : Screen, IHandle<EvtAirSample>
    {
        private IEventAggregator m_eventAggregator;

        public EvtAirSample NewestData { get; set; }

        public ObservableCollection<EvtAirSample> Collection { get; set; }

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


        public void Handle(EvtAirSample message)
        {
            NewestData = message;
        }
    }
}

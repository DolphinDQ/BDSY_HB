using AirMonitor.Interfaces;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.ViewModels
{
    class SiderViewModel : Screen
    {
        private IFactory m_factory;

        public SiderViewModel(IFactory factory)
        {
            m_factory = factory;
        }

        public Screen Top { get; set; }

        public Screen Content { get; set; }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            Top = m_factory.Create<UavStatusDisplayViewModel>();
            Content = m_factory.Create<AirSampleDisplayViewModel>();
        }
    }
}

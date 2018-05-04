using AirMonitor.Interfaces;
using AirMonitor.ViewModels;
using Caliburn.Micro;

namespace AirMonitor
{
    public class ShellViewModel : Screen, IShell
    {
        private IFactory m_factory;

        public ShellViewModel(IFactory factory)
        {
            m_factory = factory;
        }

        public object Sider { get; set; }

        public object Container { get; set; }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            Sider = m_factory.Create<DataDisplayViewModel>();
            Container = m_factory.Create<MapViewModel>();
        }

    }
}
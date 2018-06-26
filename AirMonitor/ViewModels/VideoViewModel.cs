using AirMonitor.Interfaces;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.ViewModels
{
    class VideoViewModel : Screen
    {
        private ICameraManager m_cameraManager;

        public VideoViewModel(ICameraManager cameraManager)
        {
            m_cameraManager = cameraManager;
        }

        public object CameraPanel { get; set; }

        public override void TryClose(bool? dialogResult = null)
        {
            base.TryClose(dialogResult);
            if (CameraPanel != null)
            {
                m_cameraManager.CloseVideo(CameraPanel);
            }
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            if (CameraPanel != null)
            {
                m_cameraManager.CloseVideo(CameraPanel);
            }
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            OnCameraPanelChanged();
        }

        public override async void Refresh()
        {
            base.Refresh();
            if (CameraPanel != null)
            {
                m_cameraManager.CloseVideo(CameraPanel);
                await Task.Delay(100);
                m_cameraManager.OpenVideo(CameraPanel);
            }
        }


        public void OnCameraPanelChanged()
        {
            if (CameraPanel != null)
            {
                m_cameraManager.OpenVideo(CameraPanel);
            }
        }


    }
}

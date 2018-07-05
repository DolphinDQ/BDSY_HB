using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.ViewModels
{
    class LoginViewModel : Screen
    {

        public LoginViewModel()
        {
            DisplayName = "请登录用户";
        }

        public override void TryClose(bool? dialogResult = null)
        {
            base.TryClose(true);
        }
    }
}

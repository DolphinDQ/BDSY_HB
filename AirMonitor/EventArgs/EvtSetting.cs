using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.EventArgs
{
    public class EvtSetting
    {
        public SettingCommands Command { get; set; }
        public object SettingObject { get; set; }
    }

    public enum SettingCommands
    {
        /// <summary>
        /// 请求配置。
        /// </summary>
        Request,
        /// <summary>
        /// 配置变更。
        /// </summary>
        Changed,
    }
}

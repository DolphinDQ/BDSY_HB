using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Config
{
    /// <summary>
    /// 设置。
    /// </summary>
    public class MqttSetting
    {
        /// <summary>
        /// 服务地址。
        /// </summary>
        public string Host { get; set; } = "v.vvlogic.com";
        /// <summary>
        /// 服务端口。
        /// </summary>
        public int Port { get; set; } = 9001;
        /// <summary>
        /// 用户名。
        /// </summary>
        public string UserName { get; set; } = "vv";
        /// <summary>
        /// 密码。
        /// </summary>
        public string Password { get; set; } = "vv";
        /// <summary>
        /// 环境数据的订阅主题。
        /// </summary>
        public string EnvironmentTopic { get; set; } = "sy01f";
    }
}

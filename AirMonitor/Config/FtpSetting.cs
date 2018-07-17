using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Config
{
    /// <summary>
    /// FTP配置。（读取权限）
    /// </summary>
    public class FtpSetting
    {

        public bool RememberPassword { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string Account { get; set; }

        public string Password { get; set; }
        /// <summary>
        /// 用户根目录。
        /// </summary>
        public string Root { get; set; }
    }
}

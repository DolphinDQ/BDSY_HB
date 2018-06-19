using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Config
{
    /// <summary>
    /// FTP写入服务配置。
    /// </summary>
    class FtpWriteSetting : FtpSetting
    {
        /// <summary>
        /// 共享根目录。
        /// </summary>
        public string SharedRoot { get; set; }
    }
}

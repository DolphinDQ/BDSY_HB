using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Config
{
    public class FtpProvider
    {
        public FtpProvider(FtpSetting setting)
        {
            Ftp = new FtpClient(setting.Host, setting.Port, setting.Account, setting.Password);
            Ftp.Encoding = Encoding.UTF8;
            PersonalRoot = setting.Root;
            if (setting is FtpWriteSetting s)
            {
                ShardedRoot = s.SharedRoot;
            }
        }
        public IFtpClient Ftp { get; }
        public string PersonalRoot { get; }
        public string ShardedRoot { get; }
    }
}

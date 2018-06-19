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
            PersonalTempDir = CheckDir(setting.Root);
            PersonalRoot = setting.Root;
            if (setting is FtpWriteSetting s)
            {
                ShardedTempDir = s.SharedRoot;
                ShardedRoot = s.SharedRoot;
            }
        }
        private string CheckDir(string dir)
        {
            dir = AppDomain.CurrentDomain.BaseDirectory + dir;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }
        public IFtpClient Ftp { get; }
        public string PersonalTempDir { get; }
        public string ShardedTempDir { get; }
        public string PersonalRoot { get; }
        public string ShardedRoot { get; }
    }
}

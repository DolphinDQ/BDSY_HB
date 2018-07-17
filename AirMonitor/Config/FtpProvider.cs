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
            var client = new FtpClient(setting.Host, setting.Port, setting.Account, setting.Password);
            client.Encoding = Encoding.UTF8;
            client.DataConnectionEncryption = false;
            client.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            client.EncryptionMode = FtpEncryptionMode.Explicit;
            client.ValidateCertificate += Ftp_ValidateCertificate;
            Ftp = client;
            PersonalRoot = setting.Root;
            ShardedRoot = setting.SharedRoot;
        }

        private void Ftp_ValidateCertificate(FtpClient control, FtpSslValidationEventArgs e)
        {
            e.Accept = true;
        }

        public IFtpClient Ftp { get; }
        public string PersonalRoot { get; }
        public string ShardedRoot { get; }
    }
}

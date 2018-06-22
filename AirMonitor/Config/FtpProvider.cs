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
            client.DataConnectionEncryption = true;
            client.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            client.EncryptionMode = FtpEncryptionMode.Explicit;
            client.ValidateCertificate += Ftp_ValidateCertificate;
            //client.PlainTextEncryption = true;
            //Ftp.DataConnectionType = FtpDataConnectionType.PASV;
            //client.Connect();
            Ftp = client;
            PersonalRoot = setting.Root;
            if (setting is FtpWriteSetting s)
            {
                ShardedRoot = s.SharedRoot;
            }
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

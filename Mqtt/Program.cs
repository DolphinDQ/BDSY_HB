using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mqtt
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MqttFactory().CreateMqttClient();
            var conn = client.ConnectAsync(BuildMqttClientOptions("v.vvlogic.com", 9001)).Result;
            Console.WriteLine(conn.IsSessionPresent);
            Console.WriteLine(string.Join("\n", client.SubscribeAsync("sy01f").Result.Select(o => JsonConvert.SerializeObject(o))));
            client.ApplicationMessageReceived += Client_ApplicationMessageReceived;
            Console.Read();
        }

        private static void Client_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine(JsonConvert.SerializeObject(e));
            Console.WriteLine(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
        }

        private static IMqttClientOptions BuildMqttClientOptions(string server, int? port)
        {
            return new MqttClientOptions()
            {
                ClientId = "123456",
                ProtocolVersion = MQTTnet.Serializer.MqttProtocolVersion.V311,
                Credentials = new MqttClientCredentials()
                {
                    Username = "vv",
                    Password = "vv"
                },
                CleanSession = false,
                WillMessage = new MqttApplicationMessageBuilder()
                    .WithTopic("NDEATH")
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                    .WithRetainFlag(true)
                    .Build(),
                KeepAlivePeriod = TimeSpan.FromSeconds(15),
                CommunicationTimeout = TimeSpan.FromSeconds(5),
                KeepAliveSendInterval = TimeSpan.FromSeconds(5),
                ChannelOptions = new MqttClientTcpOptions()
                {
                    Server = server,
                    Port = port,

                    BufferSize = 4096,
                    TlsOptions = new MqttClientTlsOptions()
                    {
                        AllowUntrustedCertificates = true,
                        IgnoreCertificateChainErrors = true,
                        IgnoreCertificateRevocationErrors = true,
                        UseTls = false,
                    }
                },
            };
        }
    }
}

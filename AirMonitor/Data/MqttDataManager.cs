using AirMonitor.Config;
using AirMonitor.Interfaces;
using Caliburn.Micro;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Data
{
    class MqttDataManager : IDataManager
    {
        private IEventAggregator m_eventAggregator;
        private IMqttClient m_client;
        private MqttSetting m_setting;

        public MqttDataManager(
            IEventAggregator eventAggregator,
            IConfigManager configManager)
        {
            m_eventAggregator = eventAggregator;
            m_setting = configManager.GetConfig<MqttSetting>();
        }

        public void Dispose()
        {
            m_client?.Dispose();
        }

        public async void Init()
        {
            m_client = new MqttFactory().CreateMqttClient();
            m_client.Disconnected += OnDisconnected;
            m_client.Connected += OnConnected;
            m_client.ApplicationMessageReceived += OnReceived;
            await DoConnect();
        }

        private async void OnReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            string message = e.ApplicationMessage.ConvertPayloadToString();
            this.Info("On Received:{0}", message);
            if (e.ApplicationMessage.Topic == m_setting.EnvironmentTopic)
            {
              await  m_eventAggregator.PublishOnUIThreadAsync(
                    JsonConvert.DeserializeObject<EnvironmentCallback>(message));
            }
        }

        private async void OnConnected(object sender, MqttClientConnectedEventArgs e)
        {
            this.Info("MQTT server {0}:{1} connected.", m_setting.Host, m_setting.Port);
            await Subscribe(m_setting.EnvironmentTopic);
        }

        private async Task DoConnect()
        {
            var options = new MqttClientOptions()
            {
                ClientId = Guid.NewGuid().ToString(),
                ProtocolVersion = MQTTnet.Serializer.MqttProtocolVersion.V311,
                Credentials = new MqttClientCredentials()
                {
                    Username = m_setting.UserName,
                    Password = m_setting.Password
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
                    Server = m_setting.Host,
                    Port = m_setting.Port,
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
            try
            {
                await m_client.ConnectAsync(options);
            }
            catch (Exception e)
            {
                this.Error(e);
            }
        }

        private async void OnDisconnected(object sender, MqttClientDisconnectedEventArgs e)
        {
            this.Warn("MQTT server was disconnected." + e.ClientWasConnected);
            this.Error(e.Exception);
            await Task.Delay(3000);
            await DoConnect();
        }

        private async Task Subscribe(string topic)
        {
            try
            {
                var re = await m_client.SubscribeAsync(topic);
                var fail = re.Where(o => o.ReturnCode == MqttSubscribeReturnCode.Failure);
                if (fail.Any())
                {
                    this.Warn("subscribe topic {0} failed!", string.Join(",", fail.Select(o => o.TopicFilter.Topic)));
                }
                else
                {
                    this.Warn("subscribe topic {0} success!", topic);
                }
            }
            catch (Exception e)
            {
                this.Error(e);
            }
           
        }
    }
}

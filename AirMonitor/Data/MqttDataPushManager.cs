using AirMonitor.Config;
using AirMonitor.EventArgs;
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
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class MqttDataPushManager : IDataPushManager
        , IHandle<EvtSampling>
        , IHandle<EvtSetting>
    {
        private IEventAggregator m_eventAggregator;
        private IMqttClient m_client;
        private MqttSetting m_setting;
        private bool CanPush { get; set; } = true;
        public bool IsConnected { get; set; }

        public MqttDataPushManager(
            IEventAggregator eventAggregator,
            IConfigManager configManager)
        {
            m_eventAggregator = eventAggregator;
            m_eventAggregator.Subscribe(this);
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

        private void OnReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            string message = e.ApplicationMessage.ConvertPayloadToString();
            this.Info("On Received:{0}", message);
            IsConnected = true;
            if (e.ApplicationMessage.Topic == m_setting.EnvironmentTopic)
            {
                var info = JsonConvert.DeserializeObject<EvtAirSample>(message);
                if (info.hight > 1000)
                {
                    this.Warn("recevied invald data {0}", message);
                    return;
                }
                if (CanPush)
                {
                    m_eventAggregator.PublishOnBackgroundThread(info);
                }
            }
        }

        private async void OnConnected(object sender, MqttClientConnectedEventArgs e)
        {
            this.Info("MQTT server {0}:{1} connected.", m_setting.Host, m_setting.Port);
            IsConnected = true;
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
            IsConnected = false;
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

        public void Handle(EvtSampling message)
        {

            switch (message.Status)
            {
                case SamplingStatus.StartSim:
                    CanPush = false;
                    break;
                case SamplingStatus.StopSim:
                    CanPush = true;
                    break;
                default:
                    break;
            }
        }

        public async void Handle(EvtSetting message)
        {
            if (message.Command == SettingCommands.Changed)
            {
                if (message.SettingObject is MqttSetting s)
                {
                    if (m_client != null)
                    {
                        m_setting = s;
                        await m_client.DisconnectAsync();
                    }
                }
            }
        }
    }
}

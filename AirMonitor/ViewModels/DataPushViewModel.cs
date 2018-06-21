using AirMonitor.Data;
using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using AirMonitor.Config;
using PropertyChanged;
using AirMonitor.Chart;
using AirMonitor.Camera;

namespace AirMonitor.ViewModels
{
    class DataPushViewModel : Screen,
        IHandle<EvtAirSample>,
        IHandle<EvtSetting>,
        IHandle<EvtCameraConnect>,
        IHandle<EvtCameraGetDevices>,
        IHandle<EvtSampling>
    {
        [AddINotifyPropertyChangedInterface]
        public class SampleChart
        {
            private IChartManager m_chart;

            public SampleChart(IChartManager chart, double max = double.NaN, double min = double.NaN)
            {
                Collection = new ObservableCollection<Tuple<DateTime, double>>();
                ChartModel = chart.CreateLiner(Collection, new LinerOptions() { MaxY = max, MinY = min });
                Collection.CollectionChanged += Collection_CollectionChanged;
                m_chart = chart;
            }

            private void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                if (Collection.Any())
                {
                    ActualMax = Collection.Max(o => o.Item2);
                    ActualMin = Collection.Min(o => o.Item2);
                    ActualAvg = Collection.Average(o => o.Item2);
                }
                else
                {
                    ActualMax = 0;
                    ActualMin = 0;
                    ActualAvg = 0;
                }
                IsWarning = ActualMax > MaxValue;//|| ActualMin < MinValue
            }

            public ObservableCollection<Tuple<DateTime, double>> Collection { get; private set; }

            public object ChartModel { get; private set; }

            public AirPollutant Pollutant { get; set; }

            [DependsOn(nameof(Pollutant))]
            public string Unit => Pollutant?.Unit;

            [DependsOn(nameof(Pollutant))]
            public double MaxValue => Pollutant?.MaxValue ?? 0;

            [DependsOn(nameof(Pollutant))]
            public double MinValue => Pollutant?.MinValue ?? 0;

            public double ActualMax { get; private set; }

            public double ActualMin { get; private set; }

            public double ActualAvg { get; private set; }

            public bool IsWarning { get; private set; }
        }

        private IEventAggregator m_eventAggregator;

        private IResourceManager m_res;

        public IDataManager DataManager { get; }
        public ICameraManager CameraManager { get; }
        public EvtAirSample NewestData { get; set; }

        public bool EnableSampling { get; set; }

        public double CorrectAltitude { get; set; }
        /// <summary>
        /// 数据名称列表，是采样数据的名称列表。
        /// </summary>
        public List<Tuple<string, string>> DataNameList { get; set; }

        public object CameraPanel { get; set; }

        public bool ShowVideo { get; set; }

        #region Sample list
        public SampleChart Temperature => Plots[nameof(EvtAirSample.temp)];
        public SampleChart Humidity => Plots[nameof(EvtAirSample.humi)];
        public SampleChart VOC => Plots[nameof(EvtAirSample.voc)];
        public SampleChart CO => Plots[nameof(EvtAirSample.co)];
        public SampleChart SO2 => Plots[nameof(EvtAirSample.so2)];
        public SampleChart NO2 => Plots[nameof(EvtAirSample.no2)];
        public SampleChart O3 => Plots[nameof(EvtAirSample.o3)];
        public SampleChart PM2_5 => Plots[nameof(EvtAirSample.pm25)];
        public SampleChart PM10 => Plots[nameof(EvtAirSample.pm10)];
        public SampleChart RelativeHeight => Plots[nameof(EvtAirSample.RelativeHeight)];
        public Dictionary<string, SampleChart> Plots { get; set; }

        private IConfigManager m_configManager;
        public AirStandardSetting StandardSetting { get; private set; }
        public CameraSetting CameraSetting { get; private set; }
        public bool IsCameraOnline { get; private set; }
        #endregion

        public DataPushViewModel(
            IEventAggregator eventAggregator,
            IChartManager chartManager,
            IConfigManager configManager,
            ICameraManager cameraManager,
            IDataManager data,
            IResourceManager res)
        {
            eventAggregator.Subscribe(this);
            m_eventAggregator = eventAggregator;
            m_res = res;
            DataManager = data;
            m_configManager = configManager;
            CameraManager = cameraManager;
            StandardSetting = configManager.GetConfig<AirStandardSetting>();
            CameraSetting = configManager.GetConfig<CameraSetting>();
            CorrectAltitude = StandardSetting.CorrectAltitude;
            Plots = new Dictionary<string, SampleChart>();
            Plots.Add(nameof(EvtAirSample.RelativeHeight), new SampleChart(chartManager)
            {
                Pollutant = GetHeightPollutant()
            });
            foreach (var item in StandardSetting.Pollutant)
            {
                Plots.Add(item.Name, new SampleChart(chartManager) { Pollutant = item });
            }
            CameraManager.Reconnect();
        }

        private AirPollutant GetHeightPollutant()
        {
            return new AirPollutant()
            {
                Name = nameof(EvtAirSample.RelativeHeight),
                DisplayName = m_res.GetText("T_RelativeHeight"),
                Unit = StandardSetting.AltitudeUnit,
                Levels = new[] { new AirPollutantLevel() { MinValue = 0, MaxValue = StandardSetting.MaxAltitude } },
            };
        }

        public override void TryClose(bool? dialogResult = null)
        {
            base.TryClose(dialogResult);
            m_eventAggregator.Unsubscribe(this);
        }

        public void OnEnableSamplingChanged()
        {
            m_eventAggregator.PublishOnBackgroundThread(new EvtSampling()
            {
                Status = EnableSampling ? SamplingStatus.Start : SamplingStatus.Stop
            });
        }

        public void Handle(EvtAirSample message)
        {
            message.RelativeHeight = message.hight - StandardSetting.CorrectAltitude;
            NewestData = message;
            if (EnableSampling)
            {
                FillChart(Temperature, Tuple.Create(message.RecordTime, message.temp));
                FillChart(Humidity, Tuple.Create(message.RecordTime, message.humi));
                FillChart(VOC, Tuple.Create(message.RecordTime, message.voc));
                FillChart(CO, Tuple.Create(message.RecordTime, message.co));
                FillChart(SO2, Tuple.Create(message.RecordTime, message.so2));
                FillChart(NO2, Tuple.Create(message.RecordTime, message.no2));
                FillChart(O3, Tuple.Create(message.RecordTime, message.o3));
                FillChart(PM2_5, Tuple.Create(message.RecordTime, message.pm25));
                FillChart(PM10, Tuple.Create(message.RecordTime, message.pm10));
                FillChart(RelativeHeight, Tuple.Create(message.RecordTime, message.RelativeHeight));
            }
        }

        public void CorrectHeight()
        {
            if (NewestData != null)
            {
                CorrectAltitude = NewestData.hight;
            }
        }

        public void OnCorrectAltitudeChanged()
        {
            StandardSetting.CorrectAltitude = CorrectAltitude;
            m_configManager.SaveConfig(StandardSetting);
        }

        public void VideoServiceSetting()
        {
            m_eventAggregator.PublishOnBackgroundThread(new EvtSetting() { Command = SettingCommands.Request, SettingObject = CameraSetting });
        }

        public void DataServiceSetting()
        {
            m_eventAggregator.PublishOnBackgroundThread(new EvtSetting() { Command = SettingCommands.Request, SettingObject = m_configManager.GetConfig<MqttSetting>() });
        }

        public void SampleSetting()
        {
            m_eventAggregator.PublishOnBackgroundThread(new EvtSetting() { Command = SettingCommands.Request, SettingObject = StandardSetting });
        }

        public void OpenVideo()
        {
            ShowVideo = true;
            if (CameraPanel != null)
            {
                CameraManager.OpenVideo(CameraPanel);
            }
        }

        public void CloseVideo()
        {
            ShowVideo = false;
            if (CameraPanel != null)
            {
                CameraManager.CloseVideo(CameraPanel);
            }
        }

        public void ClearData(bool focus = false)
        {
            if (focus || MessageBox.Show(m_res.GetText("T_ClearReportWarning"), "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ClearChart(Temperature);
                ClearChart(Humidity);
                ClearChart(VOC);
                ClearChart(CO);
                ClearChart(SO2);
                ClearChart(NO2);
                ClearChart(O3);
                ClearChart(PM2_5);
                ClearChart(PM10);
                ClearChart(RelativeHeight);
            }
        }

        private void ClearChart(SampleChart chart) => OnUIThread(() => chart.Collection.Clear());

        private void FillChart(SampleChart chart, Tuple<DateTime, double> value) => OnUIThread(() => chart.Collection.Add(value));

        public void Handle(EvtSetting message)
        {
            try
            {
                if (message.Command == SettingCommands.Changed && message.SettingObject is AirStandardSetting setting)
                {
                    StandardSetting = setting;
                    foreach (var item in StandardSetting.Pollutant)
                    {
                        Plots[item.Name].Pollutant = null;
                        Plots[item.Name].Pollutant = item;
                    }
                    Plots[nameof(RelativeHeight)].Pollutant = GetHeightPollutant();
                    NotifyOfPropertyChange(nameof(StandardSetting));
                }
            }
            catch (Exception e)
            {
                this.Warn("handler message {0} error", JsonConvert.SerializeObject(message));
                this.Error(e);
            }
        }

        public void Handle(EvtSampling message)
        {
            switch (message.Status)
            {
                case SamplingStatus.Stop:
                    EnableSampling = false;
                    break;
                case SamplingStatus.Start:
                    EnableSampling = true;
                    break;
                case SamplingStatus.ClearAll:
                    ClearData(true);
                    break;
            }
        }

        public void Handle(EvtCameraConnect message)
        {
            NotifyOfPropertyChange(nameof(CameraManager));
        }

        public void Handle(EvtCameraGetDevices message)
        {
            if (CameraSetting != null)
            {
                var cam = message.Devices.FirstOrDefault(o => o.Id == CameraSetting.CameraId);
                if (cam != null)
                {
                    var chnl = cam.Channel.FirstOrDefault(o => o.Channel == CameraSetting.ChannelIndex);
                    if (chnl != null)
                    {
                        IsCameraOnline = chnl.IsOnline;
                        return;
                    }
                }
                VideoServiceSetting();
            }
        }
    }
}

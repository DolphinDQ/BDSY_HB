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

namespace AirMonitor.ViewModels
{
    class DataDisplayViewModel : Screen, IHandle<EvtAirSample>
    {
        public class SampleChart
        {
            public SampleChart(IChartManager chart, double max = double.NaN, double min = double.NaN)
            {
                Collection = new ObservableCollection<Tuple<DateTime, double>>();
                ChartModel = chart.CreatLiner(Collection, max, min);
            }

            public ObservableCollection<Tuple<DateTime, double>> Collection { get; private set; }

            public object ChartModel { get; private set; }

        }

        private IEventAggregator m_eventAggregator;
        private IResourceManager m_res;

        public IDataManager DataManager { get; }

        public EvtAirSample NewestData { get; set; }

        public bool EnableSampling { get; set; }

        public double CorrectAltitude { get; set; }

        /// <summary>
        /// 数据名称列表，是采样数据的名称列表。
        /// </summary>
        public List<Tuple<string, string>> DataNameList { get; set; }

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
        #endregion

        public DataDisplayViewModel(
            IEventAggregator eventAggregator,
            IChartManager chartManager,
            IConfigManager configManager,
            IDataManager data,
            IResourceManager res)
        {
            eventAggregator.Subscribe(this);
            m_eventAggregator = eventAggregator;
            m_res = res;
            DataManager = data;
            var dataNames = new[] {
                nameof(EvtAirSample.temp),
                nameof(EvtAirSample.humi),
                nameof(EvtAirSample.voc),
                nameof(EvtAirSample.co),
                nameof(EvtAirSample.so2),
                nameof(EvtAirSample.no2),
                nameof(EvtAirSample.o3),
                nameof(EvtAirSample.pm25),
                nameof(EvtAirSample.pm10),
                nameof(EvtAirSample.RelativeHeight),
            };
            Plots = new Dictionary<string, SampleChart>();
            //var standard = configManager.GetConfig<AirStandardSetting>();
            foreach (var item in dataNames)
            {
                //var pollutant = standard.Pollutant.FirstOrDefault(o => o.Name == item);
                //if (pollutant == null)
                //{
                    //this.Warn("no found pollutant {0} setting.", item);
                    Plots.Add(item, new SampleChart(chartManager));
                //}
                //else
                //{
                //    Plots.Add(item, new SampleChart(chartManager, pollutant.MaxValue, pollutant.MinValue));
                //}
            }
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
            message.RelativeHeight = message.hight - CorrectAltitude;
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

        public void ClearData()
        {
            if (MessageBox.Show(m_res.GetText("T_ClearReportWarning"), "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
                m_eventAggregator.PublishOnBackgroundThread(new EvtSampling() { Status = SamplingStatus.Clear });
            }
        }

        private void ClearChart(SampleChart chart) => chart.Collection.Clear();

        private void FillChart(SampleChart chart, Tuple<DateTime, double> value) => chart.Collection.Add(value);

    }
}

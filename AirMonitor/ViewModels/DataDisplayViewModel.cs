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

namespace AirMonitor.ViewModels
{
    class DataDisplayViewModel : Screen, IHandle<EvtAirSample>
    {
        public class SampleChart
        {
            public SampleChart(IChartManager chart)
            {
                Collection = new ObservableCollection<Tuple<DateTime, double>>();
                ChartModel = chart.CreatLiner(Collection);
            }

            public ObservableCollection<Tuple<DateTime, double>> Collection { get; private set; }

            public object ChartModel { get; private set; }

        }

        private IEventAggregator m_eventAggregator;
        private IResourceProvider m_resourceProvider;

        public EvtAirSample NewestData { get; set; }

        public bool EnableSampling { get; set; }

        public int CorrectAltitude { get; set; }

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
        public SampleChart RelativeAltitude => Plots[nameof(EvtAirSample.RelativeAltitude)];
        public Dictionary<string, SampleChart> Plots { get; set; }
        #endregion

        public DataDisplayViewModel(
            IEventAggregator eventAggregator,
            IChartManager chartManager,
            IResourceProvider resourceProvider)
        {
            eventAggregator.Subscribe(this);
            m_eventAggregator = eventAggregator;
            m_resourceProvider = resourceProvider;
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
                nameof(EvtAirSample.RelativeAltitude),
            };
            Plots = new Dictionary<string, SampleChart>();
            foreach (var item in dataNames)
            {
                Plots.Add(item, new SampleChart(chartManager));
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
            message.RelativeAltitude = message.hight - CorrectAltitude;
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
                FillChart(RelativeAltitude, Tuple.Create(message.RecordTime, message.RelativeAltitude));
            }
        }


        public void ClearData()
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
            ClearChart(RelativeAltitude);
            m_eventAggregator.PublishOnBackgroundThread(new EvtSampling() { Status = SamplingStatus.Clear });
        }

        private void ClearChart(SampleChart chart) => chart.Collection.Clear();

        private void FillChart(SampleChart chart, Tuple<DateTime, double> value) => chart.Collection.Add(value);

    }
}

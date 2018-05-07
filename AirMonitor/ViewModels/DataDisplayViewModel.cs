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
using PropertyChanged;
using System.ComponentModel;

namespace AirMonitor.ViewModels
{
    [AddINotifyPropertyChangedInterface]
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

        public EvtAirSample NewestData { get; set; }

        public bool EnableSampling { get; set; }

        #region Sample list

        public SampleChart Temperature { get; set; }
        public SampleChart Humidity { get; set; }
        public SampleChart VOC { get; set; }
        public SampleChart CO { get; set; }
        public SampleChart SO2 { get; set; }
        public SampleChart NO2 { get; set; }
        public SampleChart O3 { get; set; }
        public SampleChart PM2_5 { get; set; }
        public SampleChart PM10 { get; set; }

        public List<EvtAirSample> Samples { get; set; }
        #endregion

        public DataDisplayViewModel(
            IEventAggregator eventAggregator,
            IChartManager chartManager,
            IResourceProvider resourceProvider)
        {
            eventAggregator.Subscribe(this);
            m_eventAggregator = eventAggregator;
            DataNameList = new List<Tuple<string, string>>(new[] {
                Tuple.Create("temp",resourceProvider.GetText("T_Temperature")),
                Tuple.Create("humi",resourceProvider.GetText("T_Humidity")),
                Tuple.Create("voc",resourceProvider.GetText("T_VOC")),
                Tuple.Create("co",resourceProvider.GetText("T_CO")),
                Tuple.Create("so2",resourceProvider.GetText("T_SO2")),
                Tuple.Create("no2",resourceProvider.GetText("T_NO2")),
                Tuple.Create("o3",resourceProvider.GetText("T_O3")),
                Tuple.Create("pm25",resourceProvider.GetText("T_PM2_5")),
                Tuple.Create("pm10",resourceProvider.GetText("T_PM10")),
            });
            Samples = new List<EvtAirSample>();
            Temperature = new SampleChart(chartManager);
            Humidity = new SampleChart(chartManager);
            VOC = new SampleChart(chartManager);
            CO = new SampleChart(chartManager);
            SO2 = new SampleChart(chartManager);
            NO2 = new SampleChart(chartManager);
            O3 = new SampleChart(chartManager);
            PM2_5 = new SampleChart(chartManager);
            PM10 = new SampleChart(chartManager);

        }

        public override void TryClose(bool? dialogResult = null)
        {
            base.TryClose(dialogResult);
            m_eventAggregator.Unsubscribe(this);
        }

        //public void OnEnableSamplingChanged()
        //{
        //    if (EnableSampling)
        //    {
        //        ClearChart(Temperature);
        //        ClearChart(Humidity);
        //        ClearChart(VOC);
        //        ClearChart(CO);
        //        ClearChart(SO2);
        //        ClearChart(NO2);
        //        ClearChart(O3);
        //        ClearChart(PM2_5);
        //        ClearChart(PM10);
        //        Samples.Clear();
        //    }
        //}

        public void Handle(EvtAirSample message)
        {
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
                Samples.Add(message);
            }
        }

        private void ClearChart(SampleChart chart) =>chart.Collection.Clear();

        private void FillChart(SampleChart chart, Tuple<DateTime, double> value) => chart.Collection.Add(value);

        /// <summary>
        /// 数据名称列表，是采样数据的名称列表。
        /// </summary>
        public List<Tuple<string, string>> DataNameList { get; set; }


        /// <summary>
        /// 采样数据名称。
        /// </summary>
        public string DataName { get; set; }
    }
}

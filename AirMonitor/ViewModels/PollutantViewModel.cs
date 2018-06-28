using AirMonitor.Config;
using AirMonitor.EventArgs;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Caliburn.Micro;
using AirMonitor.Interfaces;
using System.Collections.ObjectModel;

namespace AirMonitor.ViewModels
{
    /// <summary>
    /// 污染物统计。
    /// </summary>
    public class PollutantViewModel : Screen
    {

        public PollutantViewModel(
           IChartManager chartManager)
        {
            m_samples = new ObservableCollection<EvtAirSample>();
            m_chartManager = chartManager;
        }

        public AirPollutant Standard { get; set; }
        public object ChartModel { get; private set; }
        public bool IsWarning { get; private set; }

        private readonly ObservableCollection<EvtAirSample> m_samples;
        private readonly IChartManager m_chartManager;

        public void OnStandardChanged()
        {
            if (Standard != null)
            {
                ChartModel = m_chartManager.CreateLiner(m_samples, nameof(EvtAirSample.RecordTime), Standard.Name);
            }
            else
            {
                ChartModel = null;
            }

        }

        public void Add(EvtAirSample sample)
        {
            if (Standard == null) return;
            Execute.OnUIThread(() => m_samples.Add(sample));
            var param = Expression.Parameter(typeof(EvtAirSample));
            var property = Expression.Property(param, Standard.Name);
            var lamda = (Expression<Func<EvtAirSample, double>>)Expression.Lambda(property, param);
            var collection = m_samples.AsQueryable();
            MaxValue = Queryable.Max(collection, lamda);
            MinValue = Queryable.Min(collection, lamda);
            AvgValue = Queryable.Average(collection, lamda);
            RealtimeValue = Queryable.Select(collection, lamda).Last();

            MaxColor = Standard.GetColor(MaxValue);
            AvgColor = Standard.GetColor(AvgValue);
            MinColor = Standard.GetColor(MinValue);
            RealtimeColor = Standard.GetColor(RealtimeValue);
            IsWarning = MaxValue > Standard.MaxValue;
        }

        public void Clear()
        {
            m_samples.Clear();
            MaxValue = 0;
            MinValue = 0;
            AvgValue = 0;
            RealtimeValue = 0;
        }

        public double AvgValue { get; set; }
        public string AvgColor { get; set; }

        public double MaxValue { get; set; }
        public string MaxColor { get; set; }

        public double MinValue { get; set; }
        public string MinColor { get; set; }

        public double RealtimeValue { get; set; }
        public string RealtimeColor { get; set; }
    }
}

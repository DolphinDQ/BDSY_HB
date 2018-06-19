using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;

namespace AirMonitor.ViewModels
{
    public enum SimulatorStatus
    {
        Stop,
        Running,
        Pause,
    }
    public class SimulatorViewModel : Screen
    {
        private IResourceManager m_res;
        private ISaveManager m_saveManager;
        private IEventAggregator m_eventAggregator;
        private CancellationTokenSource m_source;
        public SimulatorStatus Status { get; set; }
        public int Interval { get; set; } = 1000;
        public int CurrentIndex { get; set; }
        public int DataCount { get; set; }

        public bool IsAlert { get; set; }
        public string Path { get; private set; }

        public SimulatorViewModel(ISaveManager saveManager, IEventAggregator eventAggregator, IResourceManager res)
        {
            m_res = res;
            m_saveManager = saveManager;
            m_eventAggregator = eventAggregator;
        }

        public void Pause() => Status = SimulatorStatus.Pause;

        public void Continue() => Status = SimulatorStatus.Running;

        public async void Replay()
        {
            if (Path != null)
            {
                await OnRun();
            }
        }

        public void Stop()
        {
            Status = SimulatorStatus.Stop;
        }

        public void OnStatusChanged()
        {
            switch (Status)
            {
                case SimulatorStatus.Stop:
                    if (m_source != null && !m_source.IsCancellationRequested)
                    {
                        m_source.Cancel();
                        m_source = null;
                        CurrentIndex = 0;
                        DataCount = 0;
                        if (IsAlert)
                        {
                            MessageBox.Show(m_res.GetText("T_SimulationSessionCompleted"));
                        }
                        m_eventAggregator.BeginPublishOnUIThread(new EvtSampling() { Status = SamplingStatus.Stop });
                        m_eventAggregator.BeginPublishOnUIThread(new EvtSampling() { Status = SamplingStatus.StopSim });
                    }
                    break;
                case SimulatorStatus.Running:
                    break;
                case SimulatorStatus.Pause:
                    break;
                default:
                    break;
            }
        }

        public async void Start()
        {
            var file = m_saveManager.ShowOpenFileDialog();
            try
            {
                if (file != null && File.Exists(file))
                {
                    Path = file;
                    await OnRun();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("打开存档失败:" + e.Message);
            }
        }

        public async Task OnRun()
        {
            var data = m_saveManager.Load(Path).Samples.OrderBy(o => o.RecordTime).ToArray();
            Stop();
            try
            {
                var source = new CancellationTokenSource();
                m_source = source;
                Status = SimulatorStatus.Running;
                m_eventAggregator.BeginPublishOnUIThread(new EvtSampling() { Status = SamplingStatus.ClearAll });
                m_eventAggregator.BeginPublishOnUIThread(new EvtSampling() { Status = SamplingStatus.StartSim });
                m_eventAggregator.BeginPublishOnUIThread(new EvtSampling() { Status = SamplingStatus.Start });
                DataCount = data.Length;
                for (int i = 0; i < DataCount; i++)
                {
                    CurrentIndex = i;
                    var item = data[CurrentIndex];
                    if (m_source != source)
                    {
                        break;
                    }
                    m_eventAggregator.PublishOnBackgroundThread(item);
                    do
                    {
                        await Task.Delay(Interval < 100 ? 100 : Interval, source.Token);
                    } while (Status == SimulatorStatus.Pause);
                }
                if (source == m_source)
                {
                    Stop();
                }
            }
            catch (Exception e)
            {
                this.Warn("simulation cancelled :{0}", e);
                this.Error(e);
            }

        }
    }
}

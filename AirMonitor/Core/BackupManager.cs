using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AirMonitor.Core
{
    class BackupManager : IBackupManager
        , IHandle<EvtAirSample>
        , IHandle<EvtSampling>
    {
        private string CacheDir { get; }
        private int CacheDays { get; }
        private string Today { get; set; }
        private string CacheFilePath { get; set; }
        private CancellationTokenSource Source { get; set; }
        private BlockingCollection<EvtAirSample> Queue { get; }
        private IEventAggregator m_eventAggregator;
        private ISaveManager m_saveManager;

        public BackupManager(IEventAggregator eventAggregator, ISaveManager saveManager)
        {
            m_eventAggregator = eventAggregator;
            m_saveManager = saveManager;
            CacheDir = "cache\\";
            CacheDays = 5;
            Queue = new BlockingCollection<EvtAirSample>(new ConcurrentQueue<EvtAirSample>());
        }

        public void Dispose()
        {
            m_eventAggregator.Unsubscribe(this);
            if (Source != null)
            {
                Source.Cancel();
                Source.Dispose();
            }
        }

        public void Init()
        {
            Today = DateTime.Today.ToString("yyyy-MM-dd");
            Task.Factory.StartNew(OnWriteCache);
            m_eventAggregator.Subscribe(this);
            if (!Directory.Exists(CacheDir))
            {
                Directory.CreateDirectory(CacheDir);
            }
            var dirs = Directory.GetDirectories(CacheDir);
            if (!dirs.Contains(Today))
            {
                Directory.CreateDirectory(CacheDir + Today);
                foreach (var item in dirs)
                {
                    try
                    {
                        if (DateTime.TryParse(item, out DateTime d))
                        {
                            if (DateTime.Now - d > TimeSpan.FromDays(CacheDays))
                            {
                                Directory.Delete(CacheDir + item, true);
                            }
                        }
                        else
                        {
                            Directory.Delete(CacheDir + item, true);
                        }
                    }
                    catch (Exception e)
                    {
                        this.Warn("delete directory {0} failed:{1}", item, e.Message);
                        this.Error(e);
                    }
                }
            }
        }

        public void Handle(EvtAirSample message)
        {
            Queue.TryAdd(message);
        }


        private void OnWriteCache()
        {
            Source = new CancellationTokenSource();
            do
            {
                if (Queue.TryTake(out var message, -1, Source.Token))
                {
                    var path = CacheFilePath;
                    if (path == null) continue;
                    if (DateTime.Now - message.RecordTime > TimeSpan.FromSeconds(10)) continue;//模拟数据。
                    File.AppendAllLines(path, new[] { JsonConvert.SerializeObject(message) });
                }
            } while (!Source.IsCancellationRequested);
        }

        private async Task OnSaveToCloud(string path)
        {
            await m_saveManager.SaveToCloud(path, m_saveManager.Load(path));
        }

        public async void Handle(EvtSampling message)
        {
            switch (message.Status)
            {
                case SamplingStatus.Stop:
                    if (CacheFilePath == null) return;
                    await OnSaveToCloud(CacheFilePath);
                    CacheFilePath = null;
                    break;
                case SamplingStatus.Start:
                    var file = CacheDir + Today + "\\cache_" + DateTime.Now.ToString("hh_mm_ss") + ".air";
                    if (!File.Exists(file))
                    {
                        File.Create(file).Dispose();
                    }
                    CacheFilePath = file;
                    break;
                case SamplingStatus.ClearAll:
                    break;
                case SamplingStatus.StartSim:
                    break;
                case SamplingStatus.StopSim:
                    break;
                default:
                    break;
            }
        }
    }
}

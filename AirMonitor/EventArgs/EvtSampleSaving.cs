using AirMonitor.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.EventArgs
{
    public class EvtSampleSaving
    {
        public SaveType Type { get; set; }

        public string Name { get; set; }

        public AirSamplesSave Save { get; set; }
    }

    public enum SaveType
    {
        SaveSamples,
        SaveSamplesCompleted,
        LoadSamples,
        LoadSamplesCompleted,
        /// <summary>
        /// 如果保存数据为空则会跳过配置。
        /// </summary>
        Skipped,
    }
}

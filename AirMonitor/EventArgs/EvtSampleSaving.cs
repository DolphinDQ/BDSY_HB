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
        /// <summary>
        /// 请求保存样本。无样本。
        /// </summary>
        SaveSamplesRequest,
        /// <summary>
        /// 保存样本。携带需要保存的样本。
        /// </summary>
        SaveSamples,
        /// <summary>
        /// 保存样本完毕。携带已经保存的样本。
        /// </summary>
        SaveSamplesCompleted,
        /// <summary>
        /// 取消保存样本。
        /// </summary>
        SaveSamplesCancelled,
        /// <summary>
        /// 请求加载样本。无样本。
        /// </summary>
        LoadSamplesRequest,
        /// <summary>
        /// 确认加载样本。无样本。
        /// </summary>
        LoadSamples,
        /// <summary>
        /// 加载样本完毕。携带已经加载的样本。
        /// </summary>
        LoadSamplesCompleted,
        /// <summary>
        /// 取消加载样本。
        /// </summary>
        LoadSamplesCancelled,
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Interfaces
{
    /// <summary>
    /// 数据管理器。
    /// </summary>
    public interface IDataPushManager : IDisposable
    {
        /// <summary>
        /// 初始化。
        /// </summary>
        void Init();
        /// <summary>
        /// 连接状态。
        /// </summary>
        bool IsConnected { get; }
    }
}

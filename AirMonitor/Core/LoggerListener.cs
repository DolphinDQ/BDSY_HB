using System;
using System.Diagnostics;

namespace AirMonitor.Core
{

    public class LoggerListener : TextWriterTraceListener
    {
        private Logger.LogType m_flag;

        /// <summary>
        /// 日志输出侦听器。
        /// </summary>
        /// <param name="flag">位运算标识，0不输出，1(0001)输出Info，2(0010)输出Warn，4(0100)输出Error7(0111)输出全部日志。</param>
        public LoggerListener(Logger.LogType flag) : base($"app-log-{DateTime.Today.ToString("yyyy-MM-dd")}.log")
        {
            m_flag = flag;
        }

        public override void Write(string message)
        {
            if (CanWrite(message))
            {
                base.Write(message);
            }
        }

        private bool CanWrite(string message)
        {
            if (message.EndsWith(": 0 : ")) return false;
            if (!m_flag.HasFlag(Logger.LogType.Error) && message.Contains($"[{Logger.LogType.Error}]")) return false;
            if (!m_flag.HasFlag(Logger.LogType.Warn) && message.Contains($"[{Logger.LogType.Warn}]")) return false;
            if (!m_flag.HasFlag(Logger.LogType.Info) && message.Contains($"[{Logger.LogType.Info}]")) return false;
            return true;
        }

        public override void WriteLine(string message)
        {
            if (CanWrite(message))
            {
                base.WriteLine(message);
            }
        }
    }
}

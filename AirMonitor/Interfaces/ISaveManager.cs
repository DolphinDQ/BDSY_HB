using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonitor.Interfaces
{
    /// <summary>
    /// 存档管理器。
    /// </summary>
    public interface ISaveManager
    {
        /// <summary>
        /// 数据保存。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        void Save(string path, object data);
        /// <summary>
        /// 数据加载。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        T Load<T>(string path);

        /// <summary>
        /// 显示保存文件对话框。
        /// </summary>
        /// <returns>未选择或者取消返回null，否则放开文件路径。</returns>
        string ShowSaveFileDialog();
        /// <summary>
        /// 显示选择文件对话框。
        /// </summary>
        /// <returns></returns>
        string ShowOpenFileDialog();
    }
}

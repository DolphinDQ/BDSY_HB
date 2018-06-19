using AirMonitor.Config;
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
        void Save(string path, AirSamplesSave data);
        /// <summary>
        /// 保存到云端。
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task SaveToCloud(string filename, AirSamplesSave data, CloudRoot root, string basedir = null);
        /// <summary>
        /// 数据加载。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        AirSamplesSave Load(string path);
        /// <summary>
        /// 加载服务文件。
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        Task<AirSamplesSave> LoadFromCloud(string filename, CloudRoot root, string basedir = null);
        /// <summary>
        /// 获取云端文件列表。
        /// </summary>
        /// <returns></returns>
        Task<CloudListItem[]> GetCloudListing(CloudRoot root, string basedir=null);
        /// <summary>
        /// 删除云端文件。
        /// </summary>
        /// <returns></returns>
        Task DeleteCloud(string filename, CloudRoot root, string basedir = null);
        /// <summary>
        /// 显示保存文件对话框。
        /// </summary>
        /// <returns>未选择或者取消返回null，否则放开文件路径。</returns>
        string ShowSaveFileDialog(string name = null);
        /// <summary>
        /// 显示选择文件对话框。
        /// </summary>
        /// <returns></returns>
        string ShowOpenFileDialog(string name = null);
    }

    /// <summary>
    /// 云端根目录
    /// </summary>
    public enum CloudRoot
    {
        /// <summary>
        /// 共享目录。
        /// </summary>
        Shared,
        /// <summary>
        /// 个人目录。
        /// </summary>
        Personal,
    }
    /// <summary>
    /// 云端文件类型。
    /// </summary>
    public enum CloudFileType
    {
        File,
        Directory,
    }
    /// <summary>
    /// 云端文件/目录
    /// </summary>
    public class CloudListItem
    {
        public string Name { get; set; }

        public CloudFileType Type { get; set; }

        public double Size { get; set; }
    }
}

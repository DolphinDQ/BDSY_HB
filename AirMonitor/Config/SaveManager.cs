using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using FluentFTP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AirMonitor.Config
{
    /// <summary>
    /// 存档管理器。
    /// </summary>
    public class SaveManager : ISaveManager
    {
        //public FtpSetting Setting { get; }
        //private IFtpClient FtpRead { get; }
        //private string PersonalDir { get; }
        //private string SharedDir { get; }

        private FtpProvider Read { get; }
        private FtpProvider Write { get; }

        public SaveManager(IConfigManager configManager)
        {
            Read = new FtpProvider(configManager.GetConfig<FtpSetting>());
            Write = new FtpProvider(configManager.GetConfig<FtpWriteSetting>());
        }

        public AirSamplesSave Load(string path)
        {
            var json = File.ReadAllText(path);
            if (!TryDeserialize<AirSamplesSave>(json, out var result))
            {
                if (TryDeserialize<EvtAirSample[]>(json, out var samples))
                {
                    result = new AirSamplesSave()
                    {
                        Samples = samples
                    };
                }
            }
            return result;
        }

        private bool TryDeserialize<T>(string json, out T result)
        {
            bool i = true;
            try
            {
                result = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                i = false;
                result = default(T);
            }
            return i;
        }

        public void Save(string path, AirSamplesSave data)
        {
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(path, json);
        }

        public string ShowSaveFileDialog(string name)
        {
            var now = DateTime.Now;
            var dlg = new SaveFileDialog()
            {
                AddExtension = true,
                OverwritePrompt = true,
                FileName = name ?? now.ToString("yyyy-MM-dd hh_mm_ss"),
                DefaultExt = "air",
                Filter = "空气采样数据|*.air",
            };
            var result = dlg.ShowDialog();
            return result == DialogResult.OK ? dlg.FileName : null;
        }

        public string ShowOpenFileDialog(string name)
        {
            var dlg = new OpenFileDialog()
            {
                CheckFileExists = true,
                FileName = name,
                Filter = "空气采样数据|*.air",
                DefaultExt = "air",
            };
            var result = dlg.ShowDialog();
            return result == DialogResult.OK ? dlg.FileName : null;
        }

        private string GetTempPath(string filename, CloudRoot root, string basedir)
        {
            switch (root)
            {
                case CloudRoot.Shared:
                    return $"{Write.ShardedTempDir ?? Read.ShardedTempDir}\\{filename}"; ;
                case CloudRoot.Personal:
                    return $"{Write.PersonalTempDir ?? Read.PersonalTempDir}\\{filename}"; ;
                default:
                    return null;
            }
        }

        private string GetRemotePath(string filename, CloudRoot root, string basedir)
        {
            string path = null;
            switch (root)
            {
                case CloudRoot.Shared:
                    path = $"{Write.ShardedRoot ?? Read.ShardedRoot}";
                    break;
                case CloudRoot.Personal:
                    path = $"{Write.PersonalRoot ?? Read.PersonalRoot}";
                    break;
                default:
                    return null;
            }
            var b = string.IsNullOrEmpty(basedir) ? "" : $"/{basedir}";
            return path == null ? filename : $"{path}{b}/{filename}";
        }

        public async Task SaveToCloud(string filename, AirSamplesSave data, CloudRoot root, string basedir = null)
        {
            var path = GetTempPath(filename, root, basedir);
            using (var nux = new AutoResetEvent(true))
            {
                var action = new Action<double>(o =>
                {
                    if (o >= 100)
                    {
                        nux.Set();
                    }
                });
                var by = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
                if (await Write.Ftp.UploadAsync(by, GetRemotePath(filename, root, basedir), FtpExists.Overwrite, true, CancellationToken.None, new Progress<double>(action)))
                {
                    await Task.Run(() =>
                    {
                        nux.WaitOne(TimeSpan.FromSeconds(10));
                    });
                }
            }
        }

        public async Task<AirSamplesSave> LoadFromCloud(string filename, CloudRoot root, string basedir = null)
        {
            var path = GetTempPath(filename, root, basedir);
            using (var nux = new AutoResetEvent(true))
            {
                var action = new Action<double>(o =>
                {
                    if (o >= 100)
                    {
                        nux.Set();
                    }
                });
                if (await Read.Ftp.DownloadFileAsync(path, GetRemotePath(filename, root, basedir), true, progress: new Progress<double>(action)))
                {
                    return await Task.Run(() =>
                    {
                        nux.WaitOne();
                        return Load(path);
                    });
                }
            }
            return null;
        }

        public async Task<CloudListItem[]> GetCloudListing(CloudRoot root, string basedir = null)
        {
            var path = (root == CloudRoot.Personal ? Read.PersonalRoot : Write.ShardedRoot);
            if (basedir != null)
            {
                path += (path == null ? "" : "/") + basedir;
            }
            return await Task.Run(() =>
                Read.Ftp.GetListing(path)
                    .Where(o => o.Type == FtpFileSystemObjectType.File || o.Type == FtpFileSystemObjectType.Directory)
                    .Select(o => new CloudListItem() { Name = o.Name, Size = o.Size, Type = FtpFileSystemObjectType.File == o.Type ? CloudFileType.File : CloudFileType.Directory })
                    .ToArray());
        }

        public async Task DeleteCloud(string filename, CloudRoot root, string basedir = null)
        {
            var path = GetRemotePath(filename, root, basedir);
            await Write.Ftp.DeleteFileAsync(path);
        }

    }
}

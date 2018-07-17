using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using Caliburn.Micro;
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
    public class SaveManager : ISaveManager,
        IHandle<EvtSetting>
    {
        //public FtpSetting Setting { get; }
        //private IFtpClient FtpRead { get; }
        //private string PersonalDir { get; }
        //private string SharedDir { get; }

        private FtpProvider Provider { get; set; }

        private IConfigManager m_configManager;

        private string TempDir { get; } = "temp\\";

        public SaveManager(IConfigManager configManager)
        {
            Provider = new FtpProvider(configManager.GetConfig<FtpSetting>());
            m_configManager = configManager;//
            if (Directory.Exists(TempDir))
            {
                Directory.Delete(TempDir, true);
            }
            Directory.CreateDirectory(TempDir);
        }

        public AirSamplesSave Load(string path)
        {
            var lines = File.ReadAllLines(path);
            if (lines.Length == 1)
            {
                var json = lines[0];
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
            else
            {
                var list = new List<EvtAirSample>();
                foreach (var item in lines)
                {
                    if (TryDeserialize<EvtAirSample>(item, out var sample))
                    {
                        list.Add(sample);
                    }
                }
                return new AirSamplesSave()
                {
                    Samples = list.ToArray()
                };
            }
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
            if (data == null) return;
            if (data.Standard == null)
            {
                data.Standard = m_configManager.GetConfig<AirStandardSetting>();
            }
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

        private string GetTempPath(string filename, string basedir)
        {
            return TempDir + Guid.NewGuid();
        }

        private string GetRemotePath(string filename, string basedir)
        {
            string path = $"{ Provider.Root}";
            var b = string.IsNullOrEmpty(basedir) ? "" : $"/{basedir}";
            return path == null ? filename : $"{path}{b}/{filename}";
        }

        public async Task SaveToCloud(string filename, AirSamplesSave data, string basedir = null)
        {
            //var path = GetTempPath(filename, root, basedir);
            if (data == null) return;
            if (data.Standard == null)
            {
                data.Standard = m_configManager.GetConfig<AirStandardSetting>();
            }
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
                if (await Provider.Ftp.UploadAsync(by, GetRemotePath(filename, basedir), FtpExists.Overwrite, true, CancellationToken.None, new Progress<double>(action)))
                {
                    await Task.Run(() =>
                    {
                        nux.WaitOne(TimeSpan.FromSeconds(10));
                    });
                }
            }
        }

        public async Task<AirSamplesSave> LoadFromCloud(string filename, string basedir = null)
        {
            var path = GetTempPath(filename, basedir);
            using (var nux = new AutoResetEvent(true))
            {
                var action = new Action<double>(o =>
                {
                    if (o >= 100)
                    {
                        nux.Set();
                    }
                });
                if (await Provider.Ftp.DownloadFileAsync(path, GetRemotePath(filename, basedir), true, progress: new Progress<double>(action)))
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

        public async Task<CloudListItem[]> GetCloudListing(string basedir = null)
        {
            var path = Provider.Root;
            if (basedir != null)
            {
                path += (path == null ? "" : "/") + basedir;
            }
            return await Task.Run(() =>
                Provider.Ftp.GetListing(path)
                    .Where(o => o.Type == FtpFileSystemObjectType.File || o.Type == FtpFileSystemObjectType.Directory)
                    .Select(o => new CloudListItem() { Name = o.Name, Size = o.Size, Type = FtpFileSystemObjectType.File == o.Type ? CloudFileType.File : CloudFileType.Directory })
                    .ToArray());
        }

        public async Task DeleteCloud(string filename, string basedir = null)
        {
            var path = GetRemotePath(filename, basedir);
            await Provider.Ftp.DeleteFileAsync(path);
        }

        public void Handle(EvtSetting message)
        {
            if (message.SettingObject is FtpSetting setting)
            {
                if (message.Command == SettingCommands.Changed)
                {
                    Provider = new FtpProvider(setting);
                }
            }
        }

    }
}

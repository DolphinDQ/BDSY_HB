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
        public FtpSetting Setting { get; }
        private IFtpClient Ftp { get; }
        private string PersonalDir { get; }
        private string SharedDir { get; }

        public SaveManager(IConfigManager configManager)
        {
            Setting = configManager.GetConfig<FtpSetting>();
            Ftp = new FtpClient(Setting.Host, Setting.Port, Setting.Account, Setting.Password);
            Ftp.Encoding = Encoding.UTF8;
            PersonalDir = CheckDir(Setting.Account);
            SharedDir = CheckDir(Setting.SharedDir);
        }

        private string CheckDir(string dir)
        {
            dir = AppDomain.CurrentDomain.BaseDirectory + dir;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
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

        public async Task SaveToCloud(string filename, AirSamplesSave data)
        {
            var path = $"{PersonalDir}\\{filename}";
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
                if (await Ftp.UploadAsync(by, $"{Setting.Account}/{filename}", FtpExists.Overwrite, false, CancellationToken.None, new Progress<double>(action)))
                {
                    await Task.Run(() =>
                    {
                        nux.WaitOne(TimeSpan.FromSeconds(10));
                    });
                }
            }
        }

        public async Task SaveToShared(string filename, AirSamplesSave data)
        {
            var path = $"{SharedDir}\\{filename}";
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
                if (await Ftp.UploadAsync(by, $"{Setting.SharedDir}/{filename}", FtpExists.Overwrite, false, CancellationToken.None, new Progress<double>(action)))
                {
                    await Task.Run(() =>
                    {
                        nux.WaitOne(TimeSpan.FromSeconds(10));
                    });
                }
            }
        }

        public async Task<AirSamplesSave> LoadFromCloud(string filename)
        {
            var path = $"{PersonalDir}\\{filename}";
            using (var nux = new AutoResetEvent(true))
            {
                var action = new Action<double>(o =>
                {
                    if (o >= 100)
                    {
                        nux.Set();
                    }
                });
                if (await Ftp.DownloadFileAsync(path, $"{Setting.Account}/{filename}", true, progress: new Progress<double>(action)))
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

        public async Task<AirSamplesSave> LoadFromShared(string filename)
        {
            var path = $"{SharedDir}\\{filename}";
            using (var nux = new AutoResetEvent(true))
            {
                var action = new Action<double>(o =>
                {
                    if (o >= 100)
                    {
                        nux.Set();
                    }
                });
                if (await Ftp.DownloadFileAsync(path, $"{Setting.SharedDir}/{filename}", true, progress: new Progress<double>(action)))
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

        public async Task<string[]> GetCloudFiles()
        {
            return await Task.Run(() => Ftp.GetListing(Setting.Account).Where(o => o.Type == FtpFileSystemObjectType.File).Select(o => o.Name).ToArray());
        }

        public async Task<string[]> GetSharedFiles()
        {
            return await Task.Run(() => Ftp.GetListing(Setting.SharedDir).Where(o => o.Type == FtpFileSystemObjectType.File).Select(o => o.Name).ToArray());
        }

        public async Task DeleteCloud(string filename)
        {
            var path = $"{Setting.Account}/{filename}";
            await Ftp.DeleteFileAsync(path);
        }

        public async Task DeleteShared(string filename)
        {
            var path = $"{Setting.SharedDir}/{filename}";
            await Ftp.DeleteFileAsync(path);
        }
    }
}

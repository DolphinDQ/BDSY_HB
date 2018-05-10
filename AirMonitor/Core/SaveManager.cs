using AirMonitor.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AirMonitor.Core
{
    /// <summary>
    /// 存档管理器。
    /// </summary>
    public class SaveManager : ISaveManager
    {
        public T Load<T>(string path)
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void Save(string path, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(path, json);
        }

        public string ShowSaveFileDialog()
        {
            var now = DateTime.Now;
            var dlg = new SaveFileDialog()
            {
                AddExtension = true,
                OverwritePrompt = true,
                FileName = now.ToString("yyyy-MM-dd hh_mm_ss"),
                DefaultExt = "air",
                Filter = "空气采样数据|*.air",
            };
            var result = dlg.ShowDialog();
            return result == DialogResult.OK ? dlg.FileName : null;
        }

        public string ShowOpenFileDialog()
        {
            var dlg = new OpenFileDialog()
            {
                CheckFileExists = true,
                Filter = "空气采样数据|*.air",
                DefaultExt = "air",
            };
            var result = dlg.ShowDialog();
            return result == DialogResult.OK ? dlg.FileName : null;
        }
    }
}

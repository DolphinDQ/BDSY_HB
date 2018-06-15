using AirMonitor.Config;
using AirMonitor.EventArgs;
using AirMonitor.Interfaces;
using Caliburn.Micro;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AirMonitor.ViewModels
{
    public enum SaveLocation
    {
        Local,
        Personal,
        Shared,
    }

    public class SaveSampleViewModel : Screen
    {
        private IConfigManager m_configManager;
        private ISaveManager m_saveManager;
        private IResourceManager m_resourceManager;
        private IEventAggregator m_eventAggregator;

        public SaveSampleViewModel(
            IResourceManager resourceManager,
            ISaveManager saveManager,
            IConfigManager configManager,
            IEventAggregator eventAggregator)
        {
            m_configManager = configManager;
            m_saveManager = saveManager;
            m_resourceManager = resourceManager;
            m_eventAggregator = eventAggregator;
            SaveLocationList = new List<Tuple<string, SaveLocation>>();
            SaveLocationList.AddRange(Enum.GetValues(typeof(SaveLocation)).Cast<SaveLocation>().Select(o => Tuple.Create(m_resourceManager.GetText(o), o)));
            SaveLocation = SaveLocation.Local;
        }

        public EvtSampleSaving Evt { get; set; }

        [DependsOn(nameof(Save))]
        public AirStandardSetting Settings => Save?.Standard;

        [DependsOn(nameof(Evt))]
        public AirSamplesSave Save => Evt.Save;

        [DependsOn(nameof(Settings))]
        public AirPollutant[] Pollutants => Settings?.Pollutant;

        public AirPollutant Current { get; set; }

        /// <summary>
        /// 显示标准参数。
        /// </summary>
        public bool ShowStandard { get; set; }
        /// <summary>
        /// 是否是保存模式。否则为加载。
        /// </summary>
        public bool IsSaveMode { get; set; }
        /// <summary>
        /// 显示文件列表。
        /// </summary>
        public bool ShowFileList { get; set; }

        private IEnumerable<string> SourceFileList { get; set; }

        public IEnumerable<string> FileList { get; set; }

        public List<Tuple<string, SaveLocation>> SaveLocationList { get; }

        public SaveLocation SaveLocation { get; set; }

        public async void OnSaveLocationChanged()
        {
            if (!IsSaveMode)
            {
                switch (SaveLocation)
                {
                    case SaveLocation.Local:
                        SourceFileList = null;
                        ShowFileList = false;
                        break;
                    case SaveLocation.Personal:
                        SourceFileList = await m_saveManager.GetCloudFiles();
                        ShowFileList = true;
                        break;
                    case SaveLocation.Shared:
                        SourceFileList = await m_saveManager.GetSharedFiles();
                        ShowFileList = true;
                        break;
                    default:
                        break;
                }
                Evt.Name = null;
                Search();
            }
        }

        public void OnEvtChanged()
        {
            Init();
            switch (Evt.Type)
            {
                case SaveType.SaveSamples:
                    InitSaveSamplesMode();
                    break;
                case SaveType.LoadSamples:
                    InitLoadSamplesMode();
                    break;
            }
        }

        private void Init()
        {

        }

        private void InitSaveSamplesMode()
        {
            IsSaveMode = true;
            Evt.Save.Standard = Evt.Save.Standard ?? m_configManager.GetConfig<AirStandardSetting>();
            Current = Pollutants.FirstOrDefault();
            Save.Since = Save.Samples.Min(o => o.RecordTime);
            Save.Until = Save.Samples.Max(o => o.RecordTime);
            Save.Duration = Save.Until - Save.Since;
            Save.Acreage = (int)((Save.Samples.Max(o => o.ActualLat) - Save.Samples.Min(o => o.ActualLat)) * (Save.Samples.Max(o => o.ActualLng) - Save.Samples.Min(o => o.ActualLng)) * Math.Pow(10, 10));
            Evt.Name = Evt.Name ?? Save.Until?.ToString("yyyy_MM_dd_hh_mm_ss") ?? "";
        }

        private void OnCompleted()
        {
            m_eventAggregator.PublishOnBackgroundThread(Evt);
        }

        private void InitLoadSamplesMode()
        {
            IsSaveMode = false;
        }

        public void Confirm()
        {
            switch (SaveLocation)
            {
                case SaveLocation.Local:
                    if (IsSaveMode)
                    {
                        OnSaveToLocal();
                    }
                    else
                    {
                        OnLoadFromLocal();
                    }
                    break;
                case SaveLocation.Personal:
                    if (IsSaveMode)
                    {
                        OnSaveToPersonal();
                    }
                    else
                    {
                        OnLoadFromPersonal();
                    }
                    break;
                case SaveLocation.Shared:
                    if (IsSaveMode)
                    {
                        OnSaveToShared();
                    }
                    else
                    {
                        OnLoadFromShared();
                    }
                    break;
                default:
                    break;
            }
        }

        private async void OnLoadFromShared()
        {
            if (Evt.Name != null)
            {
                var file = await m_saveManager.LoadFromShared(Evt.Name);
                if (file != null) m_eventAggregator.PublishOnBackgroundThread(new EvtSampleSaving() { Type = SaveType.LoadSamplesCompleted, Save = file });
            }
        }

        private void OnSaveToShared()
        {

        }

        private async void OnLoadFromPersonal()
        {

            if (Evt.Name != null)
            {
                var file = await m_saveManager.LoadFromCloud(Evt.Name);
                if (file != null) m_eventAggregator.PublishOnBackgroundThread(new EvtSampleSaving() { Type = SaveType.LoadSamplesCompleted, Save = file });
            }
        }

        private void OnSaveToPersonal()
        {

        }

        private void OnSaveToLocal()
        {
            try
            {
                var file = m_saveManager.ShowSaveFileDialog(Evt.Name);
                if (file == null) return;
                m_saveManager.Save(file, Evt.Save);
                m_eventAggregator.PublishOnBackgroundThread(new EvtSampleSaving() { Type = SaveType.SaveSamplesCompleted, Save = Evt.Save });
            }
            catch (Exception e)
            {
                MessageBox.Show(m_resourceManager.GetText("T_SaveSamplesFailed") + e.Message);
            }
        }

        private void OnLoadFromLocal()
        {
            var file = m_saveManager.ShowOpenFileDialog();
            if (file != null) m_eventAggregator.PublishOnBackgroundThread(new EvtSampleSaving() { Type = SaveType.LoadSamplesCompleted, Save = m_saveManager.Load(file) });
        }

        public void Search(string text = null)
        {
            if (text == null)
            {
                FileList = SourceFileList;
            }
            else
            {
                FileList = SourceFileList?.Where(o => o.Contains(text));
            }
        }

        //string file;
        //switch (message.Type)
        //{
        //    case SaveType.SaveSample:
        //        try
        //        {
        //            file = m_saveManager.ShowSaveFileDialog();
        //            if (file == null) return;
        //            m_saveManager.Save(file, message.Data);
        //        }
        //        catch (Exception e)
        //        {
        //            MessageBox.Show(m_res.GetText("T_SaveSamplesFailed") + e.Message);
        //        }
        //        break;
        //    case SaveType.LoadSample:
        //        file = m_saveManager.ShowOpenFileDialog();
        //        if (file != null) m_eventAggregator.PublishOnBackgroundThread(new EvtSampleSaving() { Type = SaveType.LoadCompleted, Data = m_saveManager.Load(file) });
        //        break;
        //}

    }
}

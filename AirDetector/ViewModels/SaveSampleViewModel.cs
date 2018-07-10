﻿using AirMonitor.Config;
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

        [DependsOn(nameof(Evt))]
        public AirSamplesSave Save => Evt.Save;

        public bool IsLoading { get; set; }

        public string SearchText { get; set; }

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

        public IEnumerable<string> SourceDirList { get; set; }

        public IEnumerable<string> FileList { get; set; }

        public List<Tuple<string, SaveLocation>> SaveLocationList { get; }

        public SaveLocation SaveLocation { get; set; }

        public List<string> BaseDirList { get; set; } = new List<string>();

        public string BaseDir => string.Join("/", BaseDirList);

        public string SelectedDir { get; set; }

        #region OnPropertyChanged
        public void OnSearchTextChnaged()
        {
            Search();
        }

        public void OnSaveLocationChanged()
        {
            BaseDirList.Clear();
            NotifyOfPropertyChange(nameof(BaseDir));
            ReloadList();
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

        public void OnSelectedDirChanged()
        {
            if (SelectedDir == null) return;
            BaseDirList.Add(SelectedDir);
            NotifyOfPropertyChange(nameof(BaseDir));
            ReloadList();
            SelectedDir = null;
        }

        public void OnIsSaveModeChanged()
        {
            if (IsSaveMode)
            {
                SaveLocation = SaveLocation.Local;
            }
        }
        #endregion

        //public void SelectedDir(string dirName)
        //{
        //    BaseDirList.Add(dirName);
        //    NotifyOfPropertyChange(nameof(BaseDir));
        //    ReloadList();
        //}

        public void Back()
        {
            if (BaseDirList.Any())
            {
                BaseDirList.RemoveAt(BaseDirList.Count - 1);
                NotifyOfPropertyChange(nameof(BaseDir));
                ReloadList();
            }

        }

        private async void ReloadList()
        {
            switch (SaveLocation)
            {
                case SaveLocation.Local:
                    SourceFileList = null;
                    SourceDirList = null;
                    ShowFileList = false;
                    break;
                case SaveLocation.Shared:
                    var list = await m_saveManager.GetCloudListing(CloudRoot.Shared, BaseDir);
                    SourceDirList = list.Where(o => o.Type == CloudFileType.Directory).Select(o => o.Name);
                    SourceFileList = list.Where(o => o.Type == CloudFileType.File).Select(o => o.Name);
                    ShowFileList = true;
                    break;
                default:
                    break;
            }
            if (!IsSaveMode)
            {
                Evt.Name = null;
            }
            else
            {
                ShowStandard = false;
            }
            Search();
        }

        public async void DeleteSample(string item)
        {
            var ret = MessageBox.Show("您确定要删除" + m_resourceManager.GetText(SaveLocation) + "文件:" + item, "注意", MessageBoxButton.YesNo);
            if (ret == MessageBoxResult.Yes)
            {
                switch (SaveLocation)
                {
                    case SaveLocation.Shared:
                        await m_saveManager.DeleteCloud(item, CloudRoot.Shared, BaseDir);
                        ReloadList();
                        break;
                }

            }
        }

        private void Init()
        {

        }

        private void InitSaveSamplesMode()
        {
            IsSaveMode = true;
            Evt.Save.Standard = Evt.Save.Standard ?? m_configManager.GetConfig<AirStandardSetting>();
            //Current = Pollutants.FirstOrDefault();
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

        public Task LoadFile(string file) => Confirm(new[] { file });

        public async Task Confirm(IEnumerable<object> filenames = null)
        {
            try
            {
                IsLoading = true;
                if (IsSaveMode)
                {
                    switch (SaveLocation)
                    {
                        case SaveLocation.Local:
                            await OnSaveToLocal();
                            break;
                        case SaveLocation.Shared:
                            await OnSaveToShared();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (SaveLocation)
                    {
                        case SaveLocation.Local:
                            await OnLoadFromLocal();
                            break;
                        case SaveLocation.Shared:
                            await LoadFromCloud(filenames.Cast<string>(), CloudRoot.Shared);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                this.Warn("save/load item error:{0}", e);
                this.Error(e);
                throw;
            }
            finally
            {
                IsLoading = false;
            }

        }

        private async Task LoadFromCloud(IEnumerable<string> filenames, CloudRoot root)
        {
            if (filenames == null) return;
            foreach (var item in filenames)
            {
                var file = await m_saveManager.LoadFromCloud(item, root, BaseDir);
                if (file != null) m_eventAggregator.PublishOnBackgroundThread(new EvtSampleSaving() { Type = SaveType.LoadSamplesCompleted, Save = file });
            }
        }

        private async Task<bool> CheckFileName()
        {
            if (Evt.Name == null || Evt.Name == "")
            {
                MessageBox.Show("请输入文件名。");
                return false;
            }
            IEnumerable<string> fileList = null;
            switch (SaveLocation)
            {
                case SaveLocation.Shared:
                    fileList = (await m_saveManager.GetCloudListing(CloudRoot.Shared, BaseDir)).Select(o => o.Name);
                    break;
                default:
                    fileList = Enumerable.Empty<string>();
                    break;
            }
            if (fileList.Contains(Evt.Name))
            {
                var ret = MessageBox.Show("文件以存在是否覆盖？", "注意", MessageBoxButton.YesNo);
                if (ret == MessageBoxResult.No)
                {
                    return false;
                }
            }
            return true;
        }

        private async Task OnSaveToShared()
        {
            try
            {
                if (await CheckFileName())
                {
                    await m_saveManager.SaveToCloud(Evt.Name, Evt.Save, CloudRoot.Shared, BaseDir);
                    m_eventAggregator.PublishOnBackgroundThread(new EvtSampleSaving() { Type = SaveType.SaveSamplesCompleted, Name = Evt.Name, Save = Evt.Save });
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("保存失败!" + e.Message);
                throw;
            }

        }

        private async Task OnSaveToPersonal()
        {
            try
            {
                if (await CheckFileName())
                {
                    await m_saveManager.SaveToCloud(Evt.Name, Evt.Save, CloudRoot.Personal, BaseDir);
                    m_eventAggregator.PublishOnBackgroundThread(new EvtSampleSaving() { Type = SaveType.SaveSamplesCompleted, Name = Evt.Name, Save = Evt.Save });
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("保存失败!" + e.Message);
                throw;
            }
        }

        private Task OnSaveToLocal()
        {
            try
            {
                var file = m_saveManager.ShowSaveFileDialog(Evt.Name);
                if (file != null)
                {
                    m_saveManager.Save(file, Evt.Save);
                    m_eventAggregator.PublishOnBackgroundThread(new EvtSampleSaving() { Type = SaveType.SaveSamplesCompleted, Save = Evt.Save });
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(m_resourceManager.GetText("T_SaveSamplesFailed") + e.Message);
            }
            return Task.FromResult(0);
        }

        private Task OnLoadFromLocal()
        {
            var file = m_saveManager.ShowOpenFileDialog();
            if (file != null) m_eventAggregator.PublishOnBackgroundThread(new EvtSampleSaving() { Type = SaveType.LoadSamplesCompleted, Save = m_saveManager.Load(file) });
            return Task.FromResult(0);
        }

        public void Search()
        {
            var text = SearchText;
            if (string.IsNullOrWhiteSpace(text))
            {
                FileList = SourceFileList;
            }
            else
            {
                if (SourceFileList != null)
                {
                    var sp = text.Split(' ');
                    FileList = SourceFileList.Where(o => !o.Contains(text));
                    foreach (var item in sp)
                    {
                        if (item == null || item == "") continue;
                        FileList = FileList.Where(o => !o.Contains(item));
                    }
                    var ar = FileList.ToArray();
                    FileList = SourceFileList.Where(o => !ar.Contains(o));
                }
            }
        }
    }
}
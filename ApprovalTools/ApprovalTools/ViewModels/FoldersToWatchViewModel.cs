using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ApprovalTools.Approve.Properties;
using ApprovalTools.Approve.Utilities;
using JetBrains.Annotations;
using MoreLinq;
using Newtonsoft.Json;
using Ookii.Dialogs.Wpf;

namespace ApprovalTools.Approve.ViewModels
{
    public sealed class FoldersToWatchViewModel
    {
        public FoldersToWatchViewModel()
        {
            var deserializeResult = FolderSetting
                .Deserialize(Settings.Default.Folders)
                .DistinctBy(x => x.Path)
                .Where(x => Directory.Exists(x.Path))
                .Select(x => new FolderViewModel(x.Path) {IsEnabled = x.IsEnabled});
            Folders = new ObservableCollection<FolderViewModel>(deserializeResult);
        }

        [UsedImplicitly]
        public ObservableCollection<FolderViewModel> Folders { get; private set; }

        [UsedImplicitly]
        public void AddFolder()
        {
            var dlg = new VistaFolderBrowserDialog();
            if (dlg.ShowDialog() != true) return;
            Folders.Add(
                new FolderViewModel(dlg.SelectedPath)
                {
                    IsEnabled = true
                });
            Settings.Default.Folders = JsonConvert.SerializeObject(
                Folders.Select(f =>
                    new FolderSetting
                    {
                        Path = f.Path,
                        IsEnabled = f.IsEnabled
                    }));
            Settings.Default.Save();
        }
    }
}
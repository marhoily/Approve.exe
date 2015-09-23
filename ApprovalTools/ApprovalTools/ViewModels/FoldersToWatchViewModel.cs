using System.Collections.ObjectModel;
using ApprovalTools.Approve.Properties;
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
            Folders = new ObservableCollection<FolderViewModel>(
                JsonConvert.DeserializeObject<FolderViewModel[]>(
                    Settings.Default.Folders).DistinctBy(x => x.Path));
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
                    IsEnabled = true,
                    IsDirty = true
                });
            Settings.Default.Folders = JsonConvert.SerializeObject(Folders);
            Settings.Default.Save();
        }
    }
}
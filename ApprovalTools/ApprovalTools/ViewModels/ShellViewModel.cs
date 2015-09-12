using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using ApprovalTools.Approve.Properties;
using Caliburn.Micro;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Ookii.Dialogs.Wpf;

namespace ApprovalTools.Approve.ViewModels
{
    public sealed class ShellViewModel : Screen
    {
        private readonly Araxis _araxis = new Araxis();
        private readonly FileManager _fm = new FileManager();
        private List<string> _approvalsPending;
        private bool _canAraxisCompareAllFiles;
        private bool _canAraxisCompareFolders;

        public ShellViewModel()
        {
            CanAraxisCompareFolders = true;
            CanAraxisCompareAllFiles = true;

            DisplayName = "Approve";
            Folders = new ObservableCollection<FolderViewModel>(
                JsonConvert.DeserializeObject<FolderViewModel[]>(
                Settings.Default.Folders));
            
            RefreshList();
        }

        [PublicAPI]
        public bool CanAraxisCompareFolders
        {
            get { return _canAraxisCompareFolders; }
            private set
            {
                if (value == _canAraxisCompareFolders) return;
                _canAraxisCompareFolders = value;
                NotifyOfPropertyChange();
            }
        }

        [PublicAPI]
        public ObservableCollection<FolderViewModel> Folders { get; private set; }

        [PublicAPI]
        public void RefreshList()
        {

            ApprovalsPending = Folders
                .SelectMany(f => f.GetApprovalsPending())
                .ToList();
        }

        [PublicAPI]
        public List<string> ApprovalsPending
        {
            get { return _approvalsPending; }
            set
            {
                if (Equals(value, _approvalsPending)) return;
                _approvalsPending = value;
                NotifyOfPropertyChange();
            }
        }

        [PublicAPI]
        public bool CanAraxisCompareAllFiles
        {
            get { return _canAraxisCompareAllFiles; }
            set
            {
                if (value == _canAraxisCompareAllFiles) return;
                _canAraxisCompareAllFiles = value;
                NotifyOfPropertyChange();
            }
        }

        [PublicAPI]
        public void AraxisCompareAllFiles()
        {
            foreach (var diff in Folders
                .SelectMany(f => f.GetAllDifferences()))
            {
                Process.Start(Settings.Default.Araxis,
                    string.Format("\"{0}\" \"{1}\"", diff.Item1, diff.Item2));
            }
        }

        [PublicAPI]
        public void AraxisCompareFolders()
        {
            CanAraxisCompareFolders = false;

            ThreadPool.QueueUserWorkItem(c =>
            {
                _araxis.StartSession();
                foreach (var f in Folders.Where(f => f.GetAllDifferences().Any()))
                    _araxis.Compare(_fm.PutReceivedFiles(f.Path), f.Path);
                _araxis.Wait();
                CanAraxisCompareFolders = true;
            });
        }
        [PublicAPI]
        public void ApproveAll()
        {
            foreach (var diff in Folders.SelectMany(f => f.GetAllDifferences()))
            {
                File.Delete(diff.Item2);
                File.Move(diff.Item1, diff.Item2);
            }
        
            RefreshList();
        }

        [PublicAPI]
        public void RejectAll()
        {
            foreach (var diff in Folders.SelectMany(f => f.GetAllDifferences()))
            {
                File.Delete(diff.Item1);
            }
        
            RefreshList();
        }

        [PublicAPI]
        public void ApproveAllHanging()
        {
            foreach (var diff in Folders.SelectMany(f => f.GetAllHanging()))
            {
                File.Move(diff.Item1, diff.Item2);
            }

            RefreshList();
        }

        [PublicAPI]
        public void AddFolder()
        {
            var dlg = new VistaFolderBrowserDialog();
            if (dlg.ShowDialog() != true) return;
            Folders.Add(new FolderViewModel(dlg.SelectedPath) { IsEnabled = true });
            Settings.Default.Folders = JsonConvert.SerializeObject(Folders);
            Settings.Default.Save();
            RefreshList();
        }
    }
}
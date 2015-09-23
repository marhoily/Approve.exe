using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using ApprovalTools.Approve.Properties;
using Caliburn.Micro;
using JetBrains.Annotations;
using MoreLinq;
using Newtonsoft.Json;
using Ookii.Dialogs.Wpf;

namespace ApprovalTools.Approve.ViewModels
{
    public sealed class MainViewModel : Screen
    {
        private readonly Araxis _araxis = new Araxis();
        private readonly FileManager _fm = new FileManager();
        private List<DifferenceViewModel> _approvalsPending;
        private bool _canAraxisCompareAllFiles;
        private bool _canAraxisCompareFolders;

        public MainViewModel()
        {
            CanAraxisCompareFolders = true;
            CanAraxisCompareAllFiles = true;

            DisplayName = "Approve";
            Folders = new ObservableCollection<FolderViewModel>(
                JsonConvert.DeserializeObject<FolderViewModel[]>(
                    Settings.Default.Folders));

            StartTimer();
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
        public List<DifferenceViewModel> ApprovalsPending
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

        private void StartTimer()
        {
            var dispatcherTimer = new DispatcherTimer(
                TimeSpan.FromSeconds(1),
                DispatcherPriority.Background,
                OnTick,
                Dispatcher.CurrentDispatcher);
            dispatcherTimer.Start();
            GC.KeepAlive(dispatcherTimer);
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (!Folders.Any(f => f.IsDirty)) return;
            RefreshList();
            Folders.ForEach(f => f.IsDirty = false);
        }

        [PublicAPI]
        public void RefreshList()
        {
            ApprovalsPending = Folders
                .SelectMany(f => f.All)
                .ToList();
        }

        [PublicAPI]
        public void AraxisCompareAllFiles()
        {
            foreach (var diff in Folders.SelectMany(f => f.ApprovalPending))
            {
                Process.Start(Settings.Default.Araxis,
                    string.Format("\"{0}\" \"{1}\"", diff.Received, diff.Approved));
            }
        }

        [PublicAPI]
        public void AraxisCompareFolders()
        {
            CanAraxisCompareFolders = false;

            ThreadPool.QueueUserWorkItem(c =>
            {
                _araxis.StartSession();
                foreach (var f in Folders.Where(f => f.ApprovalPending.Any()))
                    _araxis.Compare(_fm.PutReceivedFiles(f.Path), f.Path);
                _araxis.Wait();
                CanAraxisCompareFolders = true;
            });
        }

        [PublicAPI]
        public void ApproveAll()
        {
            foreach (var diff in Folders.SelectMany(f => f.ApprovalPending))
                diff.Approve();
        }

        [PublicAPI]
        public void RejectAll()
        {
            foreach (var diff in Folders.SelectMany(f => f.ApprovalPending))
                diff.Reject();
        }

        [PublicAPI]
        public void ApproveAllHanging()
        {
            foreach (var diff in Folders.SelectMany(f => f.Hanging))
                diff.Approve();
        }

        [PublicAPI]
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
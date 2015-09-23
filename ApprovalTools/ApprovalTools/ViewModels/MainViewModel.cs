using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Caliburn.Micro;
using JetBrains.Annotations;
using MoreLinq;

namespace ApprovalTools.Approve.ViewModels
{
    public sealed class MainViewModel : Screen
    {
        private readonly Araxis _araxis = new Araxis();
        private readonly FileManager _fm = new FileManager();
        private List<DifferenceViewModel> _approvalsPending;
        private bool _canAraxisCompareAllFiles;
        private bool _canAraxisCompareFolders;
        private ObservableCollection<FolderViewModel> Folders { get { return FoldersToWatchViewModel.Folders; } }

        public MainViewModel()
        {
            CanAraxisCompareFolders = true;
            CanAraxisCompareAllFiles = true;
            FoldersToWatchViewModel = new FoldersToWatchViewModel();
            StartTimer();
        }

        [UsedImplicitly]
        public FoldersToWatchViewModel FoldersToWatchViewModel { get; private set; }

        [UsedImplicitly]
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


        [UsedImplicitly]
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

        [UsedImplicitly]
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

        [UsedImplicitly]
        public void RefreshList()
        {
            ApprovalsPending = Folders
                .SelectMany(f => f.All)
                .ToList();
        }

        [UsedImplicitly]
        public void AraxisCompareAllFiles()
        {
            foreach (var diff in Folders.SelectMany(f => f.ApprovalPending))
                diff.Compare();
        }

        [UsedImplicitly]
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

        [UsedImplicitly]
        public void ApproveAll()
        {
            foreach (var diff in Folders.SelectMany(f => f.ApprovalPending))
                diff.Approve();
        }

        [UsedImplicitly]
        public void RejectAll()
        {
            foreach (var diff in Folders.SelectMany(f => f.ApprovalPending))
                diff.Reject();
        }

        [UsedImplicitly]
        public void ApproveAllHanging()
        {
            foreach (var diff in Folders.SelectMany(f => f.Hanging))
                diff.Approve();
        }
    }
}
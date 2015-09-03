using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using ApprovalTools.Approve.Properties;
using Caliburn.Micro;
using JetBrains.Annotations;

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
            DisplayName = "Approve";
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
        public string RootFolder
        {
            get { return Settings.Default.RootFolder; }
            set
            {
                Settings.Default.RootFolder = value;
                Settings.Default.Save();
                NotifyOfPropertyChange();

                RefreshList();
            }
        }

        [PublicAPI]
        public void RefreshList()
        {
            if (Directory.Exists(RootFolder))
            {
                CanAraxisCompareFolders = true;
                CanAraxisCompareAllFiles = true;
                ApprovalsPending = _fm
                    .GetPendingApprovals(RootFolder)
                    .ToList();
            }
            else
            {
                ApprovalsPending = new List<string>();
                CanAraxisCompareFolders = false;
                CanAraxisCompareAllFiles = false;
            }
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

        //[PublicAPI]
        //public string DisplayName { get; set; }

        [PublicAPI]
        public void AraxisCompareAllFiles()
        {
            var receivedFiles = Directory.GetFiles(
                RootFolder, "*.received.*", SearchOption.AllDirectories);
            foreach (var received in receivedFiles)
            {
                var approved = received.Replace(".received.", ".approved.");
                if (File.Exists(approved))
                {
                    Process.Start(Settings.Default.Araxis,
                        string.Format("\"{0}\" \"{1}\"", received, approved));
                }
            }
        }

        [PublicAPI]
        public void AraxisCompareFolders()
        {
            CanAraxisCompareFolders = false;

            ThreadPool.QueueUserWorkItem(c =>
            {
                _araxis.Compare(_fm.PutReceivedFiles(RootFolder), RootFolder);
                CanAraxisCompareFolders = true;
            });
        }
        [PublicAPI]
        public void ApproveAll()
        {
            var receivedFiles = Directory.GetFiles(
                RootFolder, "*.received.*", SearchOption.AllDirectories);
            foreach (var received in receivedFiles)
            {
                var approved = received.Replace(".received.", ".approved.");
                if (File.Exists(approved))
                {
                    File.Delete(approved);
                    File.Move(received, approved);
                }
            }
            RefreshList();
        }
    }
}
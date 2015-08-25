﻿using System.Diagnostics;
using System.IO;
using System.Threading;
using ApprovalTools.Approve.Properties;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace ApprovalTools.Approve.ViewModels
{
    public sealed class ShellViewModel : PropertyChangedBase, IHaveDisplayName
    {
        private readonly Araxis _araxis = new Araxis();
        private readonly FileManager _fm = new FileManager();
        private bool _canAraxisCompareFolders;

        public ShellViewModel()
        {
            CanAraxisCompareFolders = true;
            DisplayName = "Approve";
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
            }
        }

        [PublicAPI]
        public string DisplayName { get; set; }

        [PublicAPI]
        public void AraxisCompareAllFiles()
        {
            var received = Directory.GetFiles(RootFolder, "*.received.*", SearchOption.AllDirectories);
            foreach (var file in received)
            {
                var approved = file.Replace(".received.", ".approved.");
                if (File.Exists(approved))
                {
                    Process.Start(Settings.Default.Araxis,
                        string.Format("\"{0}\" \"{1}\"", approved, file));
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
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace ApprovalTools.Approve.ViewModels
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class FolderViewModel : IDisposable
    {
        public string Path { get; private set; }
        public bool Exists { get { return Directory.Exists(Path); } }
        public bool IsEnabled { get; set; }
        public bool IsDirty { get; set; }
        private readonly FileSystemWatcher _receivedWatcher;
        private readonly FileSystemWatcher _approvedWatcher;
        private List<DifferenceViewModel> _differencesCache;

        public FolderViewModel([NotNull]string path)
        {
            Path = path;
            _receivedWatcher = FileSystemWatcher(path, "*.received.*");
            _approvedWatcher = FileSystemWatcher(path, "*.approved.*");
        }

        private FileSystemWatcher FileSystemWatcher(string path, string received)
        {
            var w = new FileSystemWatcher(path, received);
            FileSystemEventHandler handler = (s, e) =>
            {
                IsDirty = true;
                _differencesCache = null;
            };
            w.Created += handler;
            w.Deleted += handler;
            w.Renamed += (s, e) => handler(s, e);
            w.IncludeSubdirectories = true;
            w.NotifyFilter = NotifyFilters.FileName;
            w.EnableRaisingEvents = true;
            return w;
        }

        public List<DifferenceViewModel> Buffer
        {
            get
            {
                return _differencesCache ?? (_differencesCache = (
                    from received in Directory.GetFiles(
                        Path, "*.received.*", SearchOption.AllDirectories)
                    let approved = received.Replace(".received.", ".approved.")
                    select new DifferenceViewModel(received, approved)
                    ).ToList());
            }
        }

        public IEnumerable<DifferenceViewModel> All
        {
            get
            {
                return !IsEnabled || !Exists
                    ? Enumerable.Empty<DifferenceViewModel>()
                    : Buffer;
            }
        }
        public IEnumerable<DifferenceViewModel> Hanging
        {
            get
            {
                return !IsEnabled || !Exists
                    ? Enumerable.Empty<DifferenceViewModel>()
                    : Buffer.Where(x => x.IsHanging);
            }
        }
        public IEnumerable<DifferenceViewModel> ApprovalPending
        {
            get
            {
                return !IsEnabled || !Exists
                    ? Enumerable.Empty<DifferenceViewModel>()
                    : Buffer.Where(x => !x.IsHanging);
            }
        }
        public void Dispose()
        {
            _receivedWatcher.Dispose();
            _approvedWatcher.Dispose();
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context) { IsDirty = true; }
    }
}
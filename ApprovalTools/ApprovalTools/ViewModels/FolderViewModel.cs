using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace ApprovalTools.Approve.ViewModels
{
    public sealed class FolderViewModel : IDisposable
    {
        private readonly FileManager _fm = new FileManager();
        private List<DifferenceViewModel> _buffer;
        private readonly IDisposable _watcher;

        public FolderViewModel([NotNull] string path)
        {
            Path = path;
            _watcher = _fm.Watch(path,
                () =>
                {
                    IsDirty = true;
                    _buffer = null;
                });
            IsDirty = true;
        }

        public string Path { get; private set; }
        [UsedImplicitly]
        public bool Exists { get { return Directory.Exists(Path); } }
        [UsedImplicitly]
        public bool IsEnabled { get; set; }
        public bool IsDirty { get; set; }

        private IEnumerable<DifferenceViewModel> Buffer
        {
            get
            {
                if (!IsEnabled || !Exists) return Enumerable.Empty<DifferenceViewModel>();
                return _buffer ?? (_buffer = _fm.GetDifferenceViewModels(Path).ToList());
            }
        }

        public IEnumerable<DifferenceViewModel> All { get { return Buffer; } }
        public IEnumerable<DifferenceViewModel> Hanging { get { return Buffer.Where(x => x.IsHanging); } }
        public IEnumerable<DifferenceViewModel> ApprovalPending { get { return Buffer.Where(x => !x.IsHanging); } }
        public void Dispose() { _watcher.Dispose(); }
    }
}
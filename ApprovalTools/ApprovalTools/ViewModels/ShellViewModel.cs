using System.Diagnostics;
using System.IO;
using System.Threading;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace ApprovalTools.ViewModels
{
    public sealed class ShellViewModel : PropertyChangedBase, IHaveDisplayName
    {
        private const string Root = @"C:\srcroot\dpb.new";
        private readonly Araxis _araxis = new Araxis();
        private readonly FileManager _fm = new FileManager(Root);
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
        public string DisplayName { get; set; }

        [PublicAPI]
        public void AraxisCompareAllFiles()
        {
            var araxis = @"C:\Program Files\Araxis\Araxis Merge\compare.exe";
            var received = Directory.GetFiles(@"C:\srcroot\dpb.new", "*.received.*", SearchOption.AllDirectories);
            foreach (var file in received)
            {
                var approved = file.Replace(".received.", ".approved.");
                if (File.Exists(approved))
                {
                    Process.Start(araxis,
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
                _araxis.Compare(_fm.PutReceivedFiles(), Root);
                CanAraxisCompareFolders = true;
            });
        }
    }
}
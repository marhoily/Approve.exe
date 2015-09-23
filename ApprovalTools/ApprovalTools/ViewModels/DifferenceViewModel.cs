using System.Diagnostics;
using System.IO;
using ApprovalTools.Approve.Properties;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace ApprovalTools.Approve.ViewModels
{
    [DebuggerDisplay("{DisplayName}")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class DifferenceViewModel : PropertyChangedBase
    {
        public string Received { get; private set; }
        public string Approved { get; private set; }
        public bool IsHanging { get { return !File.Exists(Approved); } }
        public bool IsChecked { get; set; }
        public bool Exists { get { return File.Exists(Received); } }

        public string DisplayName
        {
            get
            {
                var fileName = Path.GetFileName(Approved);
                Debug.Assert(fileName != null);
                return fileName.Replace(".approved.", ".*.");
            }
        }

        public DifferenceViewModel(string received, string approved)
        {
            Received = received;
            Approved = approved;
        }

        public void Compare()
        {
            Process.Start(Settings.Default.Araxis,
                string.Format("\"{0}\" \"{1}\"", Received, Approved));
        }
        public void Approve()
        {
            if (IsHanging)
            {
                File.Move(Received, Approved);
            }
            else
            {
                File.Delete(Approved);
                File.Move(Received, Approved);
            }
            NotifyOfPropertyChange(() => Exists);
        }

        public override string ToString() { return DisplayName; }
        public void Reject() { File.Delete(Received); }
    }
}
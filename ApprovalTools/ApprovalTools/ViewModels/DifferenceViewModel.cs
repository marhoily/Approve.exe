using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;

namespace ApprovalTools.Approve.ViewModels
{
    [DebuggerDisplay("{DisplayName}")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class DifferenceViewModel
    {
        public string Received { get; private set; }
        public string Approved { get; private set; }
        public bool IsHanging { get { return !File.Exists(Approved); } }
        public bool IsChecked { get; set; }

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
        }

        public override string ToString() { return DisplayName; }
        public void Reject() { File.Delete(Received); }
    }
}
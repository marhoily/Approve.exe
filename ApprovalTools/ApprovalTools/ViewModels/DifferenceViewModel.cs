using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;

namespace ApprovalTools.Approve.ViewModels
{
    [DebuggerDisplay("{DisplayName}")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class DifferenceViewModel
    {
        private readonly string _received;
        private readonly string _approved;
        public bool IsHanging { get { return !File.Exists(_approved); }}
        public string DisplayName
        {
            get
            {
                var fileName = Path.GetFileName(_approved);
                Debug.Assert(fileName != null);
                return fileName.Replace(".approved.", ".*.");
            }
        }

        public DifferenceViewModel(string received, string approved)
        {
            _received = received;
            _approved = approved;
        }

        public void Approve()
        {
            if (IsHanging)
            {
                File.Move(_received, _approved);
            }
        }

        public override string ToString() { return DisplayName; }
    }
}
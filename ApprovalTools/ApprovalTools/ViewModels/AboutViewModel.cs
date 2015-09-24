using System.Reflection;
using Caliburn.Micro;

namespace ApprovalTools.Approve.ViewModels
{
    public sealed class AboutViewModel : PropertyChangedBase
    {
        private AboutViewModel() {
        }

        public static readonly AboutViewModel Instance = new AboutViewModel();
        private string _state;
        private string _lastError;

        public string Version
        {
            get
            {
                return Assembly
                    .GetExecutingAssembly()
                    .GetName().Version.ToString();
            }
        }

        public string State
        {
            get { return _state; }
            set
            {
                if (value == _state) return;
                _state = value;
                NotifyOfPropertyChange(() => State);
            }
        }

        public string LastError
        {
            get { return _lastError; }
            set
            {
                if (value == _lastError) return;
                _lastError = value;
                NotifyOfPropertyChange(() => LastError);
            }
        }
    }
}

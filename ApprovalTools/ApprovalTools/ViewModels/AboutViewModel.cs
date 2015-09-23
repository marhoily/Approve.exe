using System.Reflection;

namespace ApprovalTools.Approve.ViewModels
{
    public sealed class AboutViewModel
    {
        public string Version
        {
            get
            {
                return Assembly
                    .GetExecutingAssembly()
                    .GetName().Version.ToString();
            }
        }
    }
}

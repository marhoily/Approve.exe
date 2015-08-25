using System.Windows;
using ApprovalTools.Approve.ViewModels;
using Caliburn.Micro;

namespace ApprovalTools.Approve
{
    public class AppBootstrapper : BootstrapperBase
    {
        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}

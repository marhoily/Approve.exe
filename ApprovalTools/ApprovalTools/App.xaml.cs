using System;
using System.Threading;
using System.Threading.Tasks;
using ApprovalTools.Approve.ViewModels;
using FirstFloor.ModernUI.Presentation;
using Squirrel;

namespace ApprovalTools.Approve
{
    public partial class App
    {
        private const string Releases = @"https://raw.githubusercontent.com/marhoily/Approve.exe/master/ApprovalTools/Releases";

        public App()
        {
            InitializeComponent();
            AppearanceManager.Current.ThemeSource = AppearanceManager.DarkThemeSource;
            Task.Factory.StartNew(async () =>
            {
                AboutViewModel.Instance.State = "Updater process started";
                await Task.Delay(TimeSpan.FromSeconds(1));
                while (true)
                {
                    try
                    {
                        AboutViewModel.Instance.State = "Ready for the first check...";
                        using (var updateManager = new UpdateManager(Releases))
                        {
                            AboutViewModel.Instance.State = "Updating...";
                            await updateManager.UpdateApp();
                            AboutViewModel.Instance.State = "Updated!";
                        }
                    }
                    catch (Exception x)
                    {
                        AboutViewModel.Instance.State = x.Message;
                        return;
                    }
                    await Task.Delay(TimeSpan.FromHours(1));
                }
            });
        }
    }
}

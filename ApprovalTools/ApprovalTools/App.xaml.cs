using System;
using System.Threading;
using System.Threading.Tasks;
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
                await Task.Delay(TimeSpan.FromSeconds(1));
                while (true)
                {
                    try
                    {
                        using (var updateManager = new UpdateManager(Releases))
                            if (updateManager.IsInstalledApp)
                                await updateManager.UpdateApp();
                    }
                    catch (Exception)
                    {
                        return;
                    } 
                    await Task.Delay(TimeSpan.FromHours(1));
                }
            });
        }
    }
}

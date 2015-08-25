using System.Diagnostics;
using System.Threading;
using Merge70;

namespace ApprovalTools.ViewModels
{
    public sealed class Araxis
    {
        public void Compare(string folder1, string folder2)
        {
            Kill("merge");

            var app = new Application();

            ShowComparison(app, folder1, folder2);

            while (app.Visible) Thread.Sleep(100);
            ActivateFilter(app, "Default");
            app.Close();
        }

        private static void ShowComparison(IApplication3 app, string folder1, string folder2)
        {
            ActivateFilter(app, "Approvals");
            app.Preferences.Longs["ShowUnchanged"] = 0;
            var folderComparison = app.FolderComparison;
            while (folderComparison.Busy) Thread.Sleep(100);
            folderComparison.SetPanelTitles("approved", "received");
            folderComparison.Active = true;
            while (folderComparison.Busy) Thread.Sleep(100);
            folderComparison.Compare(folder1, folder2);
            while (folderComparison.Busy) Thread.Sleep(100);
            folderComparison.Active = true;
            app.Preferences.Longs["ShowUnchanged"] = 0;
            folderComparison.HideEmptyFolders();
            folderComparison.Active = true;
            app.Visible = true;
            app.Active = true;
            app.Maximized = true;
        }

        private static void Kill(string processName)
        {
            var processesByName = Process.GetProcessesByName(processName);
            foreach (var process in processesByName)
                process.Kill();
        }
     
        private static void ActivateFilter(IApplication3 app, string approvals)
        {
            for (var i = 0; i < app.Preferences.Filters.Count; i++)
            {
                if (app.Preferences.Filters[i].Name == approvals)
                {
                    app.Preferences.Filters.MakeActive(i);
                    break;
                }
            }
        }
 
    }
}
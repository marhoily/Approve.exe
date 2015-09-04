using System.Diagnostics;
using System.Threading;
using Merge70;

namespace ApprovalTools.Approve.ViewModels
{
    public sealed class Araxis
    {
        private Application _app;

        public void StartSession()
        {
            Kill("merge");
            _app = new Application();
        }
        public void Compare(string folder1, string folder2)
        {
            ActivateFilter(_app, "Approvals");
            _app.Preferences.Longs["ShowUnchanged"] = 0;
            var folderComparison = _app.FolderComparison;
            while (folderComparison.Busy) Thread.Sleep(100);
            folderComparison.SetPanelTitles("approved", "received");
            folderComparison.Active = true;
            while (folderComparison.Busy) Thread.Sleep(100);
            folderComparison.Compare(folder1, folder2);
            while (folderComparison.Busy) Thread.Sleep(100);
            folderComparison.Active = true;
            _app.Preferences.Longs["ShowUnchanged"] = 0;
            folderComparison.HideEmptyFolders();
            folderComparison.Active = true;
            _app.Visible = true;
            _app.Active = true;
            _app.Maximized = true;
        }

        public void Wait()
        {
            while (_app.Visible) Thread.Sleep(100);
            ActivateFilter(_app, "Default");
            _app.Close();
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ApprovalTools.Approve.ViewModels
{
    public sealed class FileManager
    {
        public string PutReceivedFiles(string root)
        {
            var tmp = CreateTempFolder();

            var received = Directory.GetFiles(root, "*.received.*", SearchOption.AllDirectories);
            foreach (var file in received)
            {
                var approved = file.Replace(".received.", ".approved.");
                if (!File.Exists(approved)) continue;
                var target = approved.Replace(root, tmp);
                CreateDirectory(Path.GetDirectoryName(target));
                File.Copy(file, target);
            }
            return tmp;
        }
        private static string CreateTempFolder()
        {
            var tmp = Path.GetTempFileName();
            File.Delete(tmp);
            Directory.CreateDirectory(tmp);
            return tmp;
        }
        private static void CreateDirectory(string target)
        {
            if (Directory.Exists(target)) return;
            CreateDirectory(Path.GetDirectoryName(target));
            Directory.CreateDirectory(target);
        }

        public IEnumerable<DifferenceViewModel> GetDifferenceViewModels(string path)
        {
            return
                from received in Directory.GetFiles(path, "*.received.*", SearchOption.AllDirectories)
                let approved = received.Replace(".received.", ".approved.")
                select new DifferenceViewModel(received, approved);
        }

        public IDisposable Watch(string path, Action onSomethingChanged)
        {
            var receivedWatcher = CreateFsw(path, "*.received.*", onSomethingChanged);
            var approvedWatcher = CreateFsw(path, "*.approved.*", onSomethingChanged);
            return new AutoDisposable(() =>
            {
                receivedWatcher.Dispose();
                approvedWatcher.Dispose();
            });

        }
        private static FileSystemWatcher CreateFsw(string path, string received, Action onSomethingChanged)
        {
            var w = new FileSystemWatcher(path, received);
            w.Created += (s, e) => onSomethingChanged();
            w.Deleted += (s, e) => onSomethingChanged();
            w.Renamed += (s, e) => onSomethingChanged();
            w.IncludeSubdirectories = true;
            w.NotifyFilter = NotifyFilters.FileName;
            w.EnableRaisingEvents = true;
            return w;
        }
    }
}
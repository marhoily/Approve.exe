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

        public IEnumerable<DifferenceViewModel> GetPendingApprovals(string root)
        {
            return
                from received in Directory.GetFiles(root, "*.received.*", SearchOption.AllDirectories)
                let approved = received.Replace(".received.", ".approved.")
                where File.Exists(approved)
                select new DifferenceViewModel(received, approved);
        }
    }
}
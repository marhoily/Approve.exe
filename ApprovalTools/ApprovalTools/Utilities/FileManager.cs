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

        public IEnumerable<string> GetPendingApprovals(string root)
        {
            var received = Directory.GetFiles(root, "*.received.*", SearchOption.AllDirectories);
            return from file in received
                select file.Replace(".received.", ".approved.")
                into approved
                where File.Exists(approved)
                select Path.GetFileName(approved).Replace(".approved.", ".*.");
        }
    }
}
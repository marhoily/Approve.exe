using System.Diagnostics;
using System.IO;

namespace ApprovalTools.ViewModels
{
    public sealed class FileManager
    {
        private readonly string _root;

        public FileManager(string root)
        {
            _root = root;
        }

        private static string CreateTempFolder()
        {
            var tmp = Path.GetTempFileName();
            File.Delete(tmp);
            Directory.CreateDirectory(tmp);
            return tmp;
        }

        public string PutReceivedFiles()
        {
            var tmp = CreateTempFolder();

            var received = Directory.GetFiles(_root, "*.received.*", SearchOption.AllDirectories);
            foreach (var file in received)
            {
                var approved = file.Replace(".received.", ".approved.");
                if (!File.Exists(approved)) continue;
                var target = approved.Replace(_root, tmp);
                CreateDirectory(Path.GetDirectoryName(target));
                File.Copy(file, target);
            }
            return tmp;
        }

        private static void CreateDirectory(string target)
        {
            if (Directory.Exists(target)) return;
            CreateDirectory(Path.GetDirectoryName(target));
            Directory.CreateDirectory(target);
        }
    }
}
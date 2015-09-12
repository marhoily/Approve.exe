using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ApprovalTools.Approve.ViewModels
{
    public class FolderViewModel
    {
        public string Path { get; private set; }
        public bool Exists { get { return Directory.Exists(Path); } }
        public bool IsEnabled { get; set; }
        private readonly FileManager _fm = new FileManager();

        public FolderViewModel(string path)
        {
            Path = path;
        }

        public IEnumerable<string> GetApprovalsPending()
        {
            return IsEnabled && Exists
                ? _fm.GetPendingApprovals(Path)
                : Enumerable.Empty<string>();
        }

        public IEnumerable<Tuple<string, string>> GetAllDifferences()
        {
            if (!IsEnabled || !Exists) return Enumerable.Empty<Tuple<string, string>>();
            var receivedFiles = Directory.GetFiles(
                Path, "*.received.*", SearchOption.AllDirectories);
            return from received in receivedFiles
                let approved = received.Replace(".received.", ".approved.")
                where File.Exists(approved)
                select Tuple.Create(received, approved);
        }

        public IEnumerable<Tuple<string, string>> GetAllHanging()
        {
            if (!IsEnabled || !Exists) return Enumerable.Empty<Tuple<string, string>>();
            var receivedFiles = Directory.GetFiles(
                Path, "*.received.*", SearchOption.AllDirectories);
            return from received in receivedFiles
                let approved = received.Replace(".received.", ".approved.")
                where !File.Exists(approved)
                select Tuple.Create(received, approved);
        }
    }
}
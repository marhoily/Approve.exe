using System;
using ApprovalTools.Approve.Properties;
using ApprovalTools.Approve.ViewModels;
using Newtonsoft.Json;

namespace ApprovalTools.Approve.Utilities
{
    public sealed class FolderSetting
    {
        public bool IsEnabled { get; set; }
        public string Path { get; set; }

        public static FolderSetting[] Deserialize(string folders)
        {
            try
            {
                return JsonConvert
                    .DeserializeObject<FolderSetting[]>(folders);
            }
            catch (Exception x)
            {
                AboutViewModel.Instance.LastError =
                    "Deserializing Folders setting:\r\n" +
                    folders + "\r\n" +
                    "Got an error: " + x.GetType().Name + "\r\n" +
                    x.Message;
                return new FolderSetting[0];
            }
        }
    }
}
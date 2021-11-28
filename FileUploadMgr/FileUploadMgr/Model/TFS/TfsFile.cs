using System.Collections.Generic;
using System.Linq;

namespace FileUploadMgr.Model.TFS
{
    internal class TfsFile
    {
        public string Path { get; }
        public ChangeInfo ChangeInfo { get; private set; }
        public List<ChangeInfo> Histroy { get; private set; } = new List<ChangeInfo>();

        private TfsFile(string path, ChangeInfo changeInfo, List<ChangeInfo> history)
        {
            Path = path;
            ChangeInfo = changeInfo;
            Histroy = history;
        }

        public static TfsFile Create(string path, ChangeInfo changeInfo, List<ChangeInfo> history)
        {
            return new TfsFile(path, changeInfo, history);
        }

        public void UpdateChangeInfo(ChangeInfo newInfo)
        {
            ChangeInfo = newInfo;
        }

        public void UpdateHistroy(IEnumerable<ChangeInfo> newHistory)
        {
            Histroy = newHistory.ToList();
        }
    }
}

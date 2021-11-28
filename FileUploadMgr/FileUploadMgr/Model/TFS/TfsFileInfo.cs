using FileUploadMgr.Resource;
using FileUploadMgr.Xml;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace FileUploadMgr.Model.TFS
{
    internal class TfsFileInfo
    {
        private List<TfsFile> _data = new List<TfsFile>();
        private TfsFile Find(string path) => _data.Find(ele => ele.Path.Equals(path));

        public bool Load(string xmlPath)
        {
            if (!File.Exists(xmlPath)) return false;
            if (!XmlUtils.TryGetXElements(xmlPath, StrItems.TfsFile, out var elements)) return false;

            foreach (var element in elements)
            {
                if (!element.TryGetValue(StrItems.Path, out var path)) return false;
                if (!element.TryGetChild(StrItems.ChangeInfo, out var xmlChangeInfo)) return false;
                if (!ChangeInfo.TryCreateFromXml(xmlChangeInfo, out var changeInfo))
                {
                    return false;
                }
                var info = TfsFile.Create(path, changeInfo, null);

                _data.Add(info);
            }
            return true;
        }

        public bool Save(string savePath)
        {
            var xml = new XElement(nameof(TfsFileInfo));
            foreach (var item in _data)
            {
                var eachMember = new XElement(StrItems.TfsFile);
                eachMember.Add(new XElement(StrItems.Path, item.Path));
                eachMember.Add(item.ChangeInfo.ToXml());
                xml.Add(eachMember);
            }
            xml.Save(savePath);
            return true;
        }


        public bool IsAlreadyUploaded(string tfsFilePath) => _data.Any(ele => ele.Path.Equals(tfsFilePath));

        public bool IsTargeItem(string tfsFilePath, Item item)
        {
            var target = Find(tfsFilePath);
            if (target == null) return false;
            return target.ChangeInfo.IsNewerItem(item);
        }

        public void UpdateHistory(string tfsFilePath, IEnumerable<Changeset> history)
        {
            var target = Find(tfsFilePath);
            foreach (var changeset in history)
            {
                if (changeset.ChangesetId > target.ChangeInfo.ChangesetId)
                {
                    target.Histroy.Add(ChangeInfo.CreateFromChgset(changeset));
                }
            }
        }

        public void UpdateChangeInfo(string tfsFilePath, Changeset latest)
        {
            _data.Find(ele => ele.Path.Equals(tfsFilePath)).UpdateChangeInfo(ChangeInfo.CreateFromChgset(latest));
        }

        public bool AddNewData(string tfsFilePath, Changeset changeset)
        {
            if (_data.Any(ele => ele.Path.Equals(tfsFilePath))) return false;
            var newData = TfsFile.Create(tfsFilePath, ChangeInfo.CreateFromChgset(changeset), null);
            _data.Add(newData);
            return true;
        }
    }
}

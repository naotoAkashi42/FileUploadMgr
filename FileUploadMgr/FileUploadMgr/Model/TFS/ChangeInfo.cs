using FileUploadMgr.Resource;
using FileUploadMgr.Xml;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Xml.Linq;

namespace FileUploadMgr.Model.TFS
{
    internal class ChangeInfo
    {
        public int ChangesetId { get; set; }
        public string Owner { get; set; }
        public DateTime CreateTime { get; set; }
        public string CommitComment { get; set; }

        private ChangeInfo(int changesetId, string owner, DateTime createTime, string commitComment)
        {
            ChangesetId = changesetId;
            Owner = owner;
            CreateTime = createTime;
            CommitComment = commitComment;
        }

        public bool IsNewerItem(Item item) => this.ChangesetId < item.ChangesetId;

        public static bool TryCreateFromXml(XElement element, out ChangeInfo result)
        {
            result = null;
            if (!element.TryGetValue<int>(StrItems.ChangesetId, out var changesetId)) return false;
            if (!element.TryGetValue(StrItems.Owner, out var owner)) return false;
            if (!element.TryGetValue<DateTime>(StrItems.CreateTime, out var createTime)) return false;
            if (!element.TryGetValue(StrItems.CommitComment, out var comment)) return false;

            result = new ChangeInfo(changesetId, owner, createTime, comment);
            return true;
        }

        public XElement ToXml()
        {
            return new XElement(nameof(ChangeInfo),
                          new XElement(StrItems.ChangesetId, ChangesetId),
                          new XElement(StrItems.Owner, Owner),
                          new XElement(StrItems.CreateTime, CreateTime.ToString()),
                          new XElement(StrItems.CommitComment, CommitComment));
        }

        public static ChangeInfo CreateFromChgset(Changeset changeset)
        {
            return new ChangeInfo(changeset.ChangesetId, changeset.Owner, changeset.CreationDate, changeset.Comment);
        }

    }
}

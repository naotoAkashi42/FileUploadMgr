using FileUploadMgr.Resource;
using FileUploadMgr.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileUploadMgr.Model
{
    internal class FileUploadSettings
    {
        public string DstRootPath { get; private set; }

        private IEnumerable<UploadFile> _data { get; set; }

        public string GetUploadDstPath(string tfsFilePath)
        {
            var target = _data.First(info => info.TfsPath.Equals(tfsFilePath));
            return Path.Combine(DstRootPath, target.DstRelativePath);
        }

        public DateTime LastUpdateTime { get; set; }

        public bool Load(string xmlPath)
        {
            if (!File.Exists(xmlPath)) return false;
            if (!XmlUtils.TryGetElement(xmlPath, StrItems.DstRootPath, out var dstRootPath)) return false;
            DstRootPath = dstRootPath.Value;

            if (!XmlUtils.TryGetXElements(xmlPath, StrItems.UploadFile, out var uploadFiles)) return false;

            var uploadFileList = new List<UploadFile>();
            foreach (var file in uploadFiles)
            {
                if (!file.TryGetValue(StrItems.TfsPath, out var tfsPath)) return false;
                if (!file.TryGetValue(StrItems.DstRerativePath, out var dstRerativePath)) return false;
                if (!file.TryGetValue(StrItems.FileName, out var fileName)) return false;

                var uploadFile = UploadFile.Create(tfsPath, dstRerativePath, fileName);
                uploadFileList.Add(uploadFile);
            }

            _data = uploadFileList;
            return true;
        }

        public string[] GetTargetTfsFileList()
        {
            return _data.Select(ele => ele.TfsPath).ToArray();
        }
    }
}

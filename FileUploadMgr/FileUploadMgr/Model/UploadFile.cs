using System.IO;

namespace FileUploadMgr.Model
{
    internal class UploadFile
    {
        public string TfsPath { get; private set; }
        public string DstRelativePath { get; private set; }
        public string FileName { get; private set; }

        private UploadFile(string tfsPath, string dstRelativePath, string fileName)
        {
            TfsPath = tfsPath;
            DstRelativePath = dstRelativePath;
            FileName = fileName;
        }

        public static UploadFile Create(string tfsPath, string dstRelativePath, string fileName)
        {
            return new UploadFile(tfsPath, dstRelativePath, fileName);
        }
    }
}

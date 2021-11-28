using FileUploadMgr.Model;
using FileUploadMgr.Model.TFS;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.IO;
using System.Linq;

namespace FileUploadMgr.Ctrl
{
    internal class FileUploader
    {
        private readonly VersionControlServer _vsServer;
        private readonly Workspace _workspace;
        public FileUploader(string serverPath, string workspacePath)
        {
            //var tfs = new TfsTeamProjectCollection(new Uri(serverPath), new VssCredentials());
            //    _vsServer = tfs.GetService<VersionControlServer>();
            //    _workspace = _vsServer.GetWorkspace(workspacePath);
            //
        }

        public bool Upload(FileUploadSettings settings, TfsFileInfo fileInfo)
        {
            settings.LastUpdateTime = DateTime.Now;
            var targetTfsFiles = settings.GetTargetTfsFileList();

            //TFS の最新ファイル取得
            _workspace.Get(targetTfsFiles, VersionSpec.Latest, RecursionType.None, GetOptions.Overwrite);


            foreach (var item in targetTfsFiles.Select((path, index) => new { path, index }))
            {
                UploadFileCore(item.path, settings, fileInfo);
            }
            return true;
        }

        private void UploadFileCore(string tfsFilePath, FileUploadSettings settings, TfsFileInfo fileInfo)
        {
            var item = _vsServer.GetItem(tfsFilePath);
            if (fileInfo.IsAlreadyUploaded(tfsFilePath))
            {
                RenewFile(tfsFilePath, settings, fileInfo, item);
            }
            else
            {
                UploadNewFile(tfsFilePath, settings, fileInfo, item);
            }

        }

        private void RenewFile(string tfsFilePath, FileUploadSettings settings, TfsFileInfo fileInfo, Item item)
        {
            if (fileInfo.IsTargeItem(tfsFilePath, item))
            {
                fileInfo.UpdateHistory(tfsFilePath, _vsServer.QueryHistory(tfsFilePath, RecursionType.None));
                fileInfo.UpdateChangeInfo(tfsFilePath, _vsServer.GetChangeset(item.ChangesetId));
                UploadFile(_workspace.GetLocalItemForServerItem(tfsFilePath), settings.GetUploadDstPath(tfsFilePath));
            }
        }

        private void UploadNewFile(string tfsFilePath, FileUploadSettings settings, TfsFileInfo fileInfo, Item item)
        {
            UploadFile(_workspace.GetLocalItemForServerItem(tfsFilePath), settings.GetUploadDstPath(tfsFilePath));
            fileInfo.AddNewData(tfsFilePath, _vsServer.GetChangeset(item.ChangesetId));
        }

        private static void UploadFile(string localPath, string dst)
        {
            try
            {
                var targetPath = Path.Combine(dst, Path.GetFileName(localPath));
                if (File.Exists(targetPath))
                {
                    RemoveReadOnlyAttribute(targetPath);
                }

                File.Copy(localPath, targetPath, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void RemoveReadOnlyAttribute(string filePath)
        {
            var fileAttribute = File.GetAttributes(filePath);
            if ((fileAttribute & FileAttributes.ReadOnly) != 0)
            {
                fileAttribute = fileAttribute & ~FileAttributes.ReadOnly;
                File.SetAttributes(filePath, fileAttribute);
            }
        }
    }
}

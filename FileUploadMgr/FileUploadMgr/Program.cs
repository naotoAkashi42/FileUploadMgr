using FileUploadMgr.Ctrl;
using FileUploadMgr.Model;
using FileUploadMgr.Model.TFS;
using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FileUploadMgr
{
    class Program
    {
        static void Main(string[] args)
        {
            //var settings = new FileUploadSettings();
            //if (!settings.Load(@"C:\Users\aksh0\OneDrive\デスクトップ\dummySetting.xml"))
            //{
            //    Console.WriteLine("失敗");
            //    Console.ReadKey();
            //}

            var info = new TfsFileInfo();
            if (!info.Load(@"C:\Users\aksh0\OneDrive\デスクトップ\dummyData.xml"))
            {
                Console.WriteLine("失敗");
                Console.ReadKey();
            }

            info.Save(@"C:\Users\aksh0\OneDrive\デスクトップ\dummy2.xml");

            //var uploader = new FileUploader("dummy", @"dummy\hoge");
            //uploader.Upload(settings, info);
        }
    }
}
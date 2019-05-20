using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DFramework.MyStorage.SDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DFramework.MyStorage.Test
{
    [TestClass]
    public class StorageClientTest
    {
        private static StorageClient client;
        private static string uploadedFileId;
        private static readonly string testFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "测试文件.txt");

        public StorageClientTest()
        {
            client = new StorageClient("http://localhost:61754/");
        }

        [TestMethod]
        public void UploadTest()
        {
            using (var stream = File.OpenRead(testFilePath))
            {
                var fileData = client.UploadAsync(stream).Result;
                Assert.IsNotNull(fileData);
                uploadedFileId = fileData.Id;
            }
        }

        [TestMethod]
        public void GetFileDataTest()
        {
            UploadTest();
            var fileData = client.GetFileData(uploadedFileId);
            Assert.IsNotNull(fileData);
            Assert.IsTrue(fileData.Id == uploadedFileId);

            var tasks = new List<Task>();
            for (var i = 0; i < 100; i++)
                tasks.Add(Task.Run(() =>
                {
                    var fileData1 = client.GetFileData(uploadedFileId);
                    Assert.IsNotNull(fileData1);
                    Assert.IsTrue(fileData1.Id == uploadedFileId);
                }));

            Task.WaitAll(tasks.ToArray());
        }

        [TestMethod]
        public void CloneTest()
        {
            UploadTest();
            var fileData = client.Clone(uploadedFileId);
            Assert.IsNotNull(fileData);
            Assert.IsTrue(fileData.Id == uploadedFileId);
        }

        [TestMethod]
        public void RemoveTest()
        {
            UploadTest();
            client.Remove(uploadedFileId);
        }

        [TestMethod]
        public void GetFileStreamTest()
        {
            var file = new FileInfo(testFilePath);
            UploadTest();
            var streamData = client.GetFileStreamAsync(uploadedFileId).Result;
            Assert.IsNotNull(streamData);
            Assert.IsTrue(streamData.Length > 0);
            Assert.IsTrue(file.Length == streamData.Length);
        }
    }
}
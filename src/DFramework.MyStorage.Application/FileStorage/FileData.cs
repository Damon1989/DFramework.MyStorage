using System;
using System.IO;

namespace DFramework.MyStorage.FileStorage
{
    public class FileData
    {
        public string Id { get; set; }
        public long Size { get; set; }
        public string PhysicalPath { get; set; }
        public string Url { get; set; }
    }

    public class FileStreamData : IDisposable
    {
        public string Id { get; set; }
        public long Length { get; set; }
        public Stream Stream { get; set; }

        public FileStreamData(string fileId, long length, Stream stream)
        {
            Id = fileId;
            Length = length;
            Stream = stream;
        }

        public void Dispose()
        {
            Stream?.Close();
        }
    }
}

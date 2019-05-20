using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;

namespace DFramework.MyStorage.FileStorage
{
    public interface IFileAppService: IApplicationService
    {
        Task<FileData> Upload(Stream stream, string md5);
        Task<FileData> GetFileDateAndAddReference(string md5);
        Task<FileData> GetFileData(string fileId);
        Task<FileData> Clone(string fileId);
        Task Remove(string fileId);

        string GetFileMD5(Stream fileStream);
    }
}

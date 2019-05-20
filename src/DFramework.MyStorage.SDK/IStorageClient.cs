using System.IO;
using System.Threading.Tasks;

namespace DFramework.MyStorage.SDK
{
    public interface IStorageClient
    {
        FileData Upload(Stream fileStream);

        Task<FileData> UploadAsync(Stream fileStream);

        FileData GetFileData(string fileId);

        Task<FileData> GetFileDataAsync(string fileId);

        void Remove(string fileId);
        Task RemoveAsync(string fileId);

        FileData Clone(string fileId);
        Task<FileData> CloneAsync(string fileId);

        FileStreamData GetFileStream(string fileId);

        Task<FileStreamData> GetFileStreamAsync(string fileId);
    }
}
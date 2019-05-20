using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Json;

namespace DFramework.MyStorage.FileStorage
{
    class FileAppService:ApplicationService,IFileAppService
    {
        private readonly IRepository<Node,string> _nodeRepository;
        private readonly IRepository<File, string> _fileRepository;
        public FileAppService(IRepository<Node, string> nodeRepository, IRepository<File, string> fileRepository)
        {
            _nodeRepository = nodeRepository;
            _fileRepository = fileRepository;
        }

        /// <summary>
        /// upload fileStream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="md5"></param>
        /// <returns></returns>
        public async Task<FileData> Upload(Stream stream, string md5)
        {
            var file=await SaveFileStream(stream, md5);
            _fileRepository.Insert(file);
            return GetFileData(file);
        }

        /// <summary>
        /// get file data and add reference
        /// </summary>
        /// <param name="md5"></param>
        /// <returns></returns>
        public async Task<FileData> GetFileDateAndAddReference(string md5)
        {
            FileData fileData = null;
            var file = await _fileRepository.FirstOrDefaultAsync(c => c.Id == md5);
            if (file!=null)
            {
                file.ReferenceCount++;
                fileData = GetFileData(file);
            }

            return fileData;
        }

        /// <summary>
        ///  get file data
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public async Task<FileData> GetFileData(string fileId)
        {
            FileData fileData = null;
            var file = await _fileRepository.FirstOrDefaultAsync(c => c.Id == fileId);
            if (file!=null)
            {
                fileData = GetFileData(file);
            }
            else
            {
                throw new StorageException(ErrorCode.FileNotExists, "文件不存在");
            }

            return fileData;

        }

        /// <summary>
        ///  clone file (add file reference count)
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public async Task<FileData> Clone(string fileId)
        {
            FileData fileData = null;
            var file = await _fileRepository.FirstOrDefaultAsync(c => c.Id == fileId);
            if (file != null)
            {
                file.ReferenceCount++;
                fileData = GetFileData(file);
            }
            else
            {
                throw new StorageException(ErrorCode.FileNotExists, "文件不存在");
            }

            return fileData;
        }

        /// <summary>
        ///  remove file (substract file reference count)
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public async Task Remove(string fileId)
        {
            var file =await _fileRepository.FirstOrDefaultAsync(c => c.Id == fileId);
            if (file!=null)
            {
                if (file.ReferenceCount>0)
                {
                    file.ReferenceCount--;
                }
            }
            else
            {
                throw new StorageException(ErrorCode.FileNotExists, "文件不存在");
            }
        }

        public string GetFileMD5(Stream fileStream)
        {
            using (var md5 = MD5.Create())
            {
                var pos = fileStream.Position;
                var md5Str = BytesToHexString(md5.ComputeHash(fileStream));
                if (fileStream.CanRead)
                {
                    fileStream.Seek(pos, SeekOrigin.Begin);
                }
                return md5Str;
            }
        }


        private static string BytesToHexString(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            var lookup32 = _lookup32;
            var result = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                var val = lookup32[bytes[i]];
                result[2 * i] = (char)val;
                result[2 * i + 1] = (char)(val >> 16);
            }
            return new string(result);
        }

        private static readonly uint[] _lookup32 = CreateLookup32();

        private static uint[] CreateLookup32()
        {
            var result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                string s = i.ToString("X2");
                result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
            }
            return result;
        }

        /// <summary>
        /// get valid node 
        /// </summary>
        /// <param name="fileLength"></param>
        /// <returns></returns>
        private async Task<Node> GetValidNode(long fileLength)
        {
            var node = (await _nodeRepository.GetAllListAsync(c =>
                    (c.Status == NodeStatus.InUsing || c.Status == NodeStatus.InMigrationIn)
                    && c.Capacity - c.FileSize > fileLength))
                .OrderByDescending(c => c.Capacity - c.FileSize)
                .FirstOrDefault();
            if (node == null)
            {
                throw new StorageException(ErrorCode.NoValidStorageNode, "there are no valid storage node ware to use");
            }

            return node;
        }

        /// <summary>
        /// save fileStream and change node file/fileCount
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="md5"></param>
        /// <returns></returns>
        private async Task<File> SaveFileStream(Stream stream, string md5)
        {
            var node = await GetValidNode(stream.Length);
            var nodeConfig = node.Config.FromJsonString<NodeConfig>();
            var physicalDirectory = Path.Combine(nodeConfig.PhysicalHost, nodeConfig.RootPath,
                DateTime.Now.ToString("yyyy/MM/dd"));
            var physicalPath = Path.Combine(physicalDirectory, md5);
            var file = new File()
            {
                Id = md5,
                Node = node,
                Size = stream.Length,
                ReferenceCount = 1,
                Path = physicalPath.Substring(nodeConfig.PhysicalHost.Length),

            };
            node.FileSize += file.Size;
            node.FileCount++;

            if (!Directory.Exists(physicalDirectory))
            {
                Directory.CreateDirectory(physicalDirectory);
            }

            using (var fileStream=new FileStream(physicalPath,FileMode.Create))
            {
                await stream.CopyToAsync(fileStream).ConfigureAwait(false);
            }

            return file;
        }


        /// <summary>
        /// init file data
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private FileData GetFileData(File file)
        {
            return new FileData
            {
                Id = file.Id,
                Size = file.Size,
                PhysicalPath = Path.Combine(file.Node.Config.FromJsonString<NodeConfig>().PhysicalHost + file.Path),
                Url = new Uri(new Uri(file.Node.UrlHost), file.Id).ToString()
            };
        }
    }
}

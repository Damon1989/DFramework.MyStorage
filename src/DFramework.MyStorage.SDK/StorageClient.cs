using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DFramework.MyStorage.SDK
{
    public class StorageClient:IStorageClient
    {
        HttpClient StorageHttpClient { get; set; }
        static string UseUrlForcibly = ConfigurationManager.AppSettings["UseUrlForcibly"];
        static int CacheTime = int.Parse(ConfigurationManager.AppSettings["CacheTime"] ?? "2");
        static ICacheManager _cacheManager = new MemoryCacheManager();

        public StorageClient(string storageApiBaseAddress, int timeout = 3600)
        {
            StorageHttpClient = new HttpClient
            {
                Timeout = new TimeSpan(0, 0, timeout),
                BaseAddress = new Uri(storageApiBaseAddress)
            };
        }

        /// <summary>
        ///  upload fileStream
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public FileData Upload(Stream fileStream)
        {
            return UploadAsync(fileStream).Result;
        }

        /// <summary>
        ///  upload fileStream async
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public Task<FileData> UploadAsync(Stream fileStream)
        {
            if (fileStream==null)
            {
                throw new StorageException(ErrorCode.FileIsNull, "文件为空");
            }

            var md5String = GetFileMD5(fileStream);
            return StorageHttpClient.GetStringAsync("StorageApi/TryGetFileData?md5=" + md5String)
                .ContinueWith(r =>
                {
                    var result = r.Result;
                    var apiResult = JsonConvert.DeserializeObject<ApiResult<FileData>>(result);
                    var fileData = ProcessResult(apiResult);
                    if (fileData==null)
                    {
                        var streamContent = new StreamContent(fileStream);
                        return StorageHttpClient.PostAsync("StorageApi/Upload?md5=" + md5String, streamContent)
                            .ContinueWith(c =>
                            {
                                apiResult = JsonConvert.DeserializeObject<ApiResult<FileData>>(c.Result.Content
                                    .ReadAsStringAsync().Result);
                                return ProcessResult(apiResult);
                            });
                    }
                    else
                    {
                        return Task.FromResult(fileData);
                    }
                }).Unwrap();
        }

        /// <summary>
        ///  get file data
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public FileData GetFileData(string fileId)
        {
            return GetFileDataAsync(fileId).Result;
        }

        /// <summary>
        ///  get file data async
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public Task<FileData> GetFileDataAsync(string fileId)
        {
            return _cacheManager.GetAsync<FileData>(fileId, CacheTime, () =>
            
                StorageHttpClient.GetStringAsync("StorageApi/GetFileData?fileId=" + fileId)
                    .ContinueWith<FileData>(r =>
                    {
                        var apiResult = JsonConvert.DeserializeObject<ApiResult<FileData>>(r.Result);
                        return ProcessResult(apiResult);
                    })
            );
        }

        /// <summary>
        ///  remove file
        /// </summary>
        /// <param name="fileId"></param>
        public void Remove(string fileId)
        {
            RemoveAsync(fileId).Wait();
        }

        /// <summary>
        ///  remove file async
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public Task RemoveAsync(string fileId)
        {
            return StorageHttpClient.PostAsync("StorageApi/Remove", new FormUrlEncodedContent(
                new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("fileId", fileId)
                })).ContinueWith(r =>
            {
                var apiResult = JsonConvert.DeserializeObject<ApiResult>(r.Result.Content.ReadAsStringAsync().Result);
                ProcessResult(apiResult);
            });
        }

        /// <summary>
        ///  clone file
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public FileData Clone(string fileId)
        {
            return CloneAsync(fileId).Result;
        }

        /// <summary>
        ///  clone file async
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public Task<FileData> CloneAsync(string fileId)
        {
            return StorageHttpClient.PostAsync("StorageApi/Clone",
                new FormUrlEncodedContent(new KeyValuePair<string, string>[]{
                    new KeyValuePair<string, string>("fileId", fileId)
                })).ContinueWith<FileData>(r =>
            {
                var apiResult = JsonConvert.DeserializeObject<ApiResult<FileData>>(r.Result.Content.ReadAsStringAsync().Result);
                return ProcessResult(apiResult);
            });
        }

        /// <summary>
        ///  get fileStream
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public FileStreamData GetFileStream(string fileId)
        {
            return GetFileStreamAsync(fileId).Result;
        }

        /// <summary>
        ///  get fileStream async
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public Task<FileStreamData> GetFileStreamAsync(string fileId)
        {
            var fileData = GetFileData(fileId);
            if (fileData==null)
            {
                throw new StorageException(ErrorCode.FileNotExists, "文件不存在");
            }

            if ("true".Equals(UseUrlForcibly,StringComparison.OrdinalIgnoreCase)||string.IsNullOrWhiteSpace(fileData.PhysicalPath))
            {
                return StorageHttpClient.GetAsync(fileData.Url, HttpCompletionOption.ResponseHeadersRead)
                    .ContinueWith(r =>
                    {
                        if (r.Result.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            throw new FileNotFoundException();
                        }

                        var length = r.Result.Content.Headers.ContentLength.GetValueOrDefault(0);
                        return r.Result.Content.ReadAsStreamAsync()
                            .ContinueWith(r1 => new FileStreamData(fileId, length, r1.Result));
                    }).Unwrap();
            }
            else
            {
                if (!System.IO.File.Exists(fileData.PhysicalPath))
                {
                    throw new FileNotFoundException();
                }

                var fileStream = new FileStream(fileData.PhysicalPath, FileMode.Open, FileAccess.Read);
                return Task.FromResult(new FileStreamData(fileId, fileData.Size, fileStream));
            }
        }


        TResult ProcessResult<TResult>(ApiResult<TResult> apiResult)
        {
            if (apiResult.ErrorCode == ErrorCode.NoError.GetHashCode())
            {
                return apiResult.Result;
            }

            throw new StorageException((ErrorCode)apiResult.ErrorCode, apiResult.Message);
        }

        void ProcessResult(ApiResult apiResult)
        {
            if (apiResult.ErrorCode != ErrorCode.NoError.GetHashCode())
            {
                throw new StorageException((ErrorCode)apiResult.ErrorCode, apiResult.Message);
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
    }
}

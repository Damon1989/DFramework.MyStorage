using System.Threading.Tasks;
using System.Web.Mvc;
using DFramework.MyStorage.FileStorage;

namespace DFramework.MyStorage.Web.Controllers
{
    public class StorageApiController : Controller
    {
        private readonly IExceptionManager _exceptionManager;
        private readonly IFileAppService _fileAppService;

        public StorageApiController(IFileAppService fileAppService, IExceptionManager exceptionManager)
        {
            _fileAppService = fileAppService;
            _exceptionManager = exceptionManager;
        }

        /// <summary>
        /// get file data
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetFileData(string fileId)
        {
            var apiResult = await _exceptionManager.ProcessAsync(() => _fileAppService.GetFileData(fileId));
            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> TryGetFileData(string md5)
        {
            var apiResult = await _exceptionManager.ProcessAsync(() => _fileAppService.GetFileDateAndAddReference(md5));
            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// upload file  and check md5 with fileStream
        /// </summary>
        /// <param name="md5"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Upload(string md5)


        {
            var apiResult = await _exceptionManager.ProcessAsync(async () =>
            {
                var fileData = await _fileAppService.GetFileDateAndAddReference(md5);
                if (fileData == null)
                {
                    if (Request.InputStream == null)
                        throw new StorageException(ErrorCode.NoFileUploaded, "no file uploaded");

                    var currentMd5 = _fileAppService.GetFileMD5(Request.InputStream);
                    if (currentMd5 != md5) throw new StorageException(ErrorCode.FileNotComplete, "file not completed");

                    fileData = await _fileAppService.Upload(Request.InputStream, md5);
                }

                return fileData;
            });
            return Json(apiResult);
        }

        /// <summary>
        ///  clone file
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Clone(string fileId)
        {
            var apiResult = await _exceptionManager.ProcessAsync(() => _fileAppService.Clone(fileId));
            return Json(apiResult);
        }

        /// <summary>
        ///  remove file
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Remove(string fileId)
        {
            var apiResult = await _exceptionManager.ProcessAsync(() => _fileAppService.Remove(fileId));
            return Json(apiResult);
        }
    }
}
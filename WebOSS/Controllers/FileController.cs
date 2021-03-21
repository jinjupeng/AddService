using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetModular.Lib.Config.Abstractions;
using NetModular.Lib.Config.Abstractions.Impl;
using NetModular.Lib.OSS.Abstractions;
using NetModular.Lib.Utils.Core.Enums;
using NetModular.Lib.Utils.Mvc.Helpers;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebOSS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileStorageProvider _fileStorageProvider;
        private readonly IConfigProvider _configProvider;
        private readonly FileUploadHelper _fileUploadHelper;

        public FileController(IFileStorageProvider fileStorageProvider, IConfigProvider configProvider,
            FileUploadHelper fileUploadHelper)
        {
            _fileStorageProvider = fileStorageProvider;
            _configProvider = configProvider;
            _fileUploadHelper = fileUploadHelper;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormCollection form)
        {
            var config = _configProvider.Get<PathConfig>();
            var formFile = form.Files.FirstOrDefault();
            var uploadModel = new FileUploadModel
            {
                Request = Request,
                FormFile = formFile,
                RootPath = config.UploadPath,
                Module = "Admin",
                Group = Path.Combine("OSS", "Open"),
                SubPath = Path.Combine("code", "file")
            };

            var fileUploadResult = await _fileUploadHelper.Upload(uploadModel);
            var fileInfo = new NetModular.Lib.Utils.Core.Files.FileInfo(formFile.FileName)
            {
                SaveName = formFile.FileName,
                Path = "resource/"
            };
            var fileObj = new FileObject
            {
                PhysicalPath = Path.Combine(config.UploadPath, fileUploadResult.Data.FullPath),
                AccessMode = FileAccessMode.Private,
                Group = Path.Combine("OSS", "Open"),
                ModuleCode = "Admin",
                FileInfo = fileInfo
            };
            var fileStorageResult = _fileStorageProvider.Upload(fileObj);

            return Ok(await Task.FromResult(fileStorageResult));
        }
    }
}

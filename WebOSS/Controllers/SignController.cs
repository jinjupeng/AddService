using DigitalSignature.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetModular.Lib.Config.Abstractions;
using NetModular.Lib.Config.Abstractions.Impl;
using NetModular.Lib.Utils.Mvc.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebOSS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignController : ControllerBase
    {
        private readonly IConfigProvider _configProvider;
        private readonly FileUploadHelper _fileUploadHelper;
        private readonly IDigitalSignatureProvider _digitalSignatureProvider;

        public SignController(IConfigProvider configProvider, FileUploadHelper fileUploadHelper,
            IDigitalSignatureProvider digitalSignatureProvider)
        {
            _configProvider = configProvider;
            _fileUploadHelper = fileUploadHelper;
            _digitalSignatureProvider = digitalSignatureProvider;
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
                Group = Path.Combine("Sign", "Open"),
                SubPath = Path.Combine("code", "file"),
                CalcMd5 = true
            };

            var fileUploadResult = await _fileUploadHelper.Upload(uploadModel);
            KeyValuePair<string, string> keyPair = _digitalSignatureProvider.CreateRSAKey();
            string privateKey = keyPair.Value;
            string publicKey = keyPair.Key;
            var originalData = fileUploadResult.Data.Md5; // 将文件哈希值当作签名
            //1、生成签名，通过摘要算法
            var signedData = _digitalSignatureProvider.HashAndSignString(originalData, privateKey);

            //2、验证签名
            bool verify = _digitalSignatureProvider.VerifySigned(originalData, signedData, publicKey);

            return Ok(await Task.FromResult(verify));
        }
    }
}

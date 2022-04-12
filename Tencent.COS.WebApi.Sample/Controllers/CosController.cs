using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tencent.COS.WebApi.Sample.Services;

namespace Tencent.COS.WebApi.Sample.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CosController : ControllerBase
    {
        private readonly ICosService _cosService;

        public CosController(ICosService cosService)
        {
            _cosService = cosService;
        }

        [HttpGet]
        public void CreateBucket(string bucket)
        {
            _cosService.PutBucket(bucket);
        }
        [HttpPost]
        public async Task<string> PutFile(IFormFile file)
        {
            // 方式一：
            FileStream fs = new FileStream(file.FileName,FileMode.OpenOrCreate);
            await file.CopyToAsync(fs);
            string fileName = Guid.NewGuid() + "-" + file.FileName;
            var resultStr = _cosService.PutObject(fileName, fs);

            // 方式二：
            //string fileName = Guid.NewGuid() + "-" + file.FileName;
            //var stream = file.OpenReadStream();
            //byte[] bytes = new byte[stream.Length];
            //await stream.ReadAsync(bytes, 0, bytes.Length);
            //stream.Close();
            //var resultStr = _cosService.PutObject(fileName, bytes);
            return resultStr;

        }
        [HttpGet]
        public ActionResult GetFile(string cosKey, bool download = false)
        {
            var fileName = cosKey;
            var fileBytes = _cosService.GetObjectToByte(cosKey);
            var fileExt = Path.GetExtension(fileName);
            var contentType = CosService.GetContentType(fileExt);
            if (download)
            {
                return File(fileBytes, contentType, fileName);
            }
            return File(fileBytes, contentType);

        }

        [HttpDelete]
        public string DeleteFile(string cosKey)
        {
            return _cosService.DeleteObject(cosKey);
        }
    }
}
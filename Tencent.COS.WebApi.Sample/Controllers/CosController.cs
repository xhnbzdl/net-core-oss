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
    }
}
using AntSK.Domain.Model;
using AntSK.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AntSK.Controllers
{

    /// <summary>
    /// 对外接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OpenController : ControllerBase
    {
        /// <summary>
        /// 对话接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/chat/completions")]
        public IActionResult chat(OpenAIModel model)
        {
            
            return Ok();
        }
    }
}

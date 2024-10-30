using AntSK.Domain;
using AntSK.Domain.Domain.Model.Dto.OpenAPI;
using AntSK.Domain.Utils;
using AntSK.Services.OpenApi;
using Microsoft.AspNetCore.Mvc;

namespace AntSK.Controllers
{
    /// <summary>
    /// 对外接口
    /// </summary>
    [ApiController]
    public class OpenController(IOpenApiService _openApiService) : ControllerBase
    {
        /// <summary>
        /// 对话接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/chat/completions")]
        public async Task Chat(OpenAIModel model)
        {
            string sk = HttpContext.Request.Headers["Authorization"].ConvertToString();
            await _openApiService.Chat(model, sk, HttpContext);
        }

        [HttpPost]
        [Route("api/v1/rerank")]
        public async Task<IActionResult> Rerank(RerankModel model)
        {
            try
            {
                string sk = HttpContext.Request.Headers["Authorization"].ConvertToString();
                var result = await _openApiService.Rerank(model, sk, HttpContext);
                return Ok(result.Success());
            }
            catch (Exception ex)
            {
                return Ok(ResponseResult.Error("1001",ex.Message));
            }

        }
    }
}
using AntSK.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AntSK.Domain.Utils;
using AntSK.Services.LLamaSharp;

namespace AntSK.Controllers
{
    [ApiController]
    public class LLamaSharpController(ILLamaSharpService _lLamaSharpService) : ControllerBase
    {
        /// <summary>
        /// 本地会话接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("llama/v1/chat/completions")]
        public async Task chat(OpenAIModel model)
        {
            if (model.stream)
            {
                await _lLamaSharpService.ChatStream(model, HttpContext);
            }
            else
            {
                await _lLamaSharpService.Chat(model, HttpContext);
            }
        }

        /// <summary>
        /// 本地嵌入接口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("llama/v1/embeddings")]
        public async Task embedding(OpenAIEmbeddingModel model)
        {

            await _lLamaSharpService.Embedding(model,HttpContext);

        }
    }
}

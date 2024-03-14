using AntSK.Domain.Domain.Model.Dto.OpenAPI;
using AntSK.Services.LLamaSharp;
using Microsoft.AspNetCore.Mvc;

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
            Console.WriteLine("开始：llama/v1/chat/completions");
            if (model.stream)
            {
                await _lLamaSharpService.ChatStream(model, HttpContext);
            }
            else
            {
                await _lLamaSharpService.Chat(model, HttpContext);
            }
            Console.WriteLine("结束：llama/v1/chat/completions");
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
            Console.WriteLine("开始：llama/v1/embeddings");
            await _lLamaSharpService.Embedding(model, HttpContext);
            Console.WriteLine("结束：llama/v1/embeddings");

        }
    }
}

using AntDesign;
using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Domain.Service;
using AntSK.Domain.Options;
using AntSK.Domain.Utils;
using AntSK.Models;
using AntSK.Models.OpenAPI;
using AntSK.Services.OpenApi;
using Azure;
using DocumentFormat.OpenXml.EMMA;
using LLama;
using LLama.Common;
using Newtonsoft.Json;
using System.Text;
using System.Threading;
using static Azure.Core.HttpHeader;
using ServiceLifetime = AntSK.Domain.Common.DependencyInjection.ServiceLifetime;

namespace AntSK.Services.LLamaSharp
{

    public interface ILLamaSharpService
    {
        Task Chat(OpenAIModel model, HttpContext HttpContext);
        Task ChatStream(OpenAIModel model, HttpContext HttpContext);
        Task Embedding(OpenAIEmbeddingModel model, HttpContext HttpContext);
    }

    [ServiceDescription(typeof(ILLamaSharpService), ServiceLifetime.Scoped)]
    public class LLamaSharpService(
        ILLamaEmbeddingService _lLamaEmbeddingService,
        ILLamaChatService _lLamaChatService
        ) : ILLamaSharpService
    {
   
        public async Task ChatStream(OpenAIModel model, HttpContext HttpContext)
        {
            HttpContext.Response.ContentType = "text/event-stream";
            string questions = model.messages.LastOrDefault().content;
            await foreach (var r in _lLamaChatService.ChatStreamAsync(questions))
            {
                Console.Write(r);
                await HttpContext.Response.WriteAsync("data:" + r + "\n\n");
                await HttpContext.Response.Body.FlushAsync();
            }
            await HttpContext.Response.WriteAsync("data: [DONE]");
            await HttpContext.Response.Body.FlushAsync();
            await HttpContext.Response.CompleteAsync();
        }

        public async Task Chat(OpenAIModel model, HttpContext HttpContext)
        {
            string questions = model.messages.LastOrDefault().content;
            OpenAIResult result = new OpenAIResult();
            result.created = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            result.choices = new List<ChoicesModel>() { new ChoicesModel() { message = new OpenAIMessage() { role = "assistant" } } };

            result.choices[0].message.content =await _lLamaChatService.ChatAsync(questions); ;
            HttpContext.Response.ContentType = "application/json";
            await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(result));
            await HttpContext.Response.CompleteAsync();
        }


        public async Task Embedding(OpenAIEmbeddingModel model, HttpContext HttpContext)
        {
            var result = new OpenAIEmbeddingResult();
            result.data[0].embedding = await _lLamaEmbeddingService.Embedding(model.input[0]);
            HttpContext.Response.ContentType = "application/json";
            await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(result));
            await HttpContext.Response.CompleteAsync();
        }
    }
}

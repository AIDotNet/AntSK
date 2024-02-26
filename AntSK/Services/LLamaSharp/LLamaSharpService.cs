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
    public class LLamaSharpService : ILLamaSharpService
    {
        private ChatSession _session;
        private LLamaContext _context;
        private LLamaEmbedder _embedder;
        private bool _continue = false;
        private const string SystemPrompt = "你是一个智能助手，所有问题你都需要用中文回答。";

        private async Task InitChatModel() {
            var @params = new LLama.Common.ModelParams(LLamaSharpOption.Chat)
            {
                ContextSize = 512,
            };
            using var weights = LLamaWeights.LoadFromFile(@params);
            _context = new LLamaContext(weights, @params);

            _session = new ChatSession(new InteractiveExecutor(_context));
            _session.History.AddMessage(AuthorRole.System, SystemPrompt);

        }


        public async IAsyncEnumerable<string> SendStream(OpenAIModel model)
        {
            await InitChatModel();
            if (!_continue)
            {
            
                _continue = true;
            }
            string questions = model.messages.LastOrDefault().content;
            for (int i = 0; i < model.messages.Count() - 2; i++)
            {
                var item = model.messages[i];
                switch (item.role)
                {
                    case "User":
                        _session.History.AddMessage(AuthorRole.User, item.content);
                        break;
                    case "System":
                        _session.History.AddMessage(AuthorRole.System, item.content);
                        break;
                    case "Assistant":
                        _session.History.AddMessage(AuthorRole.Assistant, item.content);
                        break;
                }
            }
            var outputs = _session.ChatAsync(
                new ChatHistory.Message(AuthorRole.User, questions)
                , new InferenceParams()
                {
                    RepeatPenalty = 0.3f,
                });

            await foreach (var output in outputs)
            {
                yield return output;
            }
        }
        public async Task ChatStream(OpenAIModel model, HttpContext HttpContext)
        {
            HttpContext.Response.ContentType = "text/event-stream";

            await foreach (var r in SendStream(model))
            {
                await HttpContext.Response.WriteAsync("data:" + r + "\n\n");
                await HttpContext.Response.Body.FlushAsync();
            }

            await HttpContext.Response.CompleteAsync();
        }

        public async Task Chat(OpenAIModel model, HttpContext HttpContext)
        {
            string questions = model.messages.LastOrDefault().content;
            for (int i = 0; i < model.messages.Count() - 2; i++)
            {
                var item = model.messages[i];        
                switch (item.role)
                {
                    case "User":
                        _session.History.AddMessage(AuthorRole.User, item.content);
                        break;
                    case "System":
                        _session.History.AddMessage(AuthorRole.System, item.content);
                        break;
                    case "Assistant":
                        _session.History.AddMessage(AuthorRole.Assistant, item.content);
                        break;
                }          
            }


            OpenAIResult result = new OpenAIResult();
            result.created = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            result.choices = new List<ChoicesModel>() { new ChoicesModel() { message = new OpenAIMessage() { role = "assistant" } } };

            await InitChatModel();
            if (!_continue)
            {

                _continue = true;
            }
            var outputs = _session.ChatAsync(
            new ChatHistory.Message(AuthorRole.User, questions),
            new InferenceParams()
            {
                RepeatPenalty = 0.3f,  
            });

            var llmResult = "";
            await foreach (var output in outputs)
            {
                llmResult += output;
            }
            result.choices[0].message.content = llmResult;
            HttpContext.Response.ContentType = "application/json";
            await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(result));
            await HttpContext.Response.CompleteAsync();
        }


        private async Task InitEmbeddingModel() {

            var @params = new ModelParams(LLamaSharpOption.Embedding) { EmbeddingMode = true };
            using var weights = LLamaWeights.LoadFromFile(@params);
            _embedder = new LLamaEmbedder(weights, @params);

        }

        public async Task Embedding(OpenAIEmbeddingModel model, HttpContext HttpContext)
        {
            await InitEmbeddingModel();

            float[] embeddings = _embedder.GetEmbeddings(model.input[0]).Result;
            var result = new OpenAIEmbeddingResult();
            result.data.embedding = embeddings.ToList();
            HttpContext.Response.ContentType = "application/json";
            await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(result));
            await HttpContext.Response.CompleteAsync();
        }
    }
}

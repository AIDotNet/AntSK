using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Model;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using AntSK.Models;
using AntSK.Models.OpenAPI;
using AntSK.Pages.ChatPage;
using MarkdownSharp;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System.Text;
using System;
using ServiceLifetime = AntSK.Domain.Common.DependencyInjection.ServiceLifetime;
using AntDesign.Core.Extensions;
using Azure.AI.OpenAI;
using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using AntDesign;
using Newtonsoft.Json;
using System.Text.Json;
using AntSK.Domain.Domain.Interface;
using static LLama.Common.ChatHistory;
using DocumentFormat.OpenXml.Wordprocessing;
using AntSK.Domain.Domain.Service;

namespace AntSK.Services.OpenApi
{
    public interface IOpenApiService
    {
        Task Chat(OpenAIModel model, string sk, HttpContext HttpContext);
    }

    [ServiceDescription(typeof(IOpenApiService), ServiceLifetime.Scoped)]
    public class OpenApiService(
        IApps_Repositories _apps_Repositories,
        IKmss_Repositories _kmss_Repositories,
        IKmsDetails_Repositories _kmsDetails_Repositories,
        IKernelService _kernelService,
        IKMService _kMService
        ) : IOpenApiService
    {
        public async Task Chat(OpenAIModel model,string sk, HttpContext HttpContext)
        {
            Apps app = _apps_Repositories.GetFirst(p => p.SecretKey == sk);      
            if (app.IsNotNull())
            {
                string msg= await HistorySummarize(app,model);
                switch (app.Type)
                {
                    case "chat":
                        //普通会话
                        if (model.stream)
                        {
                            OpenAIStreamResult result1 = new OpenAIStreamResult();
                            result1.created = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                            result1.choices = new List<StreamChoicesModel>() { new StreamChoicesModel() { delta = new OpenAIMessage() { role = "assistant" } } };
                            await SendChatStream( HttpContext, result1, app, msg);
                            HttpContext.Response.ContentType = "application/json";
                            await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(result1));
                            await HttpContext.Response.CompleteAsync();
                            return;
                        }
                        else 
                        {
                            OpenAIResult result2 = new OpenAIResult();
                            result2.created = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                            result2.choices = new List<ChoicesModel>() { new ChoicesModel() { message = new OpenAIMessage() { role = "assistant" } } };
                            result2.choices[0].message.content = await SendChat(msg, app);
                            HttpContext.Response.ContentType = "application/json";
                            await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(result2));
                            await HttpContext.Response.CompleteAsync();
                        }
                        break;
                    case "kms":
                        //知识库问答
                        OpenAIResult result3 = new OpenAIResult();
                        result3.created = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        result3.choices = new List<ChoicesModel>() { new ChoicesModel() { message = new OpenAIMessage() { role = "assistant" } } };
                        result3.choices[0].message.content = await SendKms( msg, app);
                        HttpContext.Response.ContentType = "application/json";
                        await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(result3));
                        await HttpContext.Response.CompleteAsync();
                        break;
                }
            }
    
        }

        private async Task SendChatStream( HttpContext HttpContext, OpenAIStreamResult result, Apps app, string msg)
        {
            var _kernel = _kernelService.GetKernelByApp(app);
            var temperature = app.Temperature / 100;//存的是0~100需要缩小
            OpenAIPromptExecutionSettings settings = new() { Temperature = temperature };
            if (!string.IsNullOrEmpty(app.ApiFunctionList))
            {
                _kernelService.ImportFunctionsByApp(app, _kernel);
                settings = new() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions, Temperature = temperature };
            }

            HttpContext.Response.Headers.Add("Content-Type", "text/event-stream");

            if (string.IsNullOrEmpty(app.Prompt) || !app.Prompt.Contains("{{$input}}"))
            {
                //如果模板为空，给默认提示词
                app.Prompt = app.Prompt.ConvertToString() + "{{$input}}";
            }
            //var promptTemplateFactory = new KernelPromptTemplateFactory();
            //var promptTemplate = promptTemplateFactory.Create(new PromptTemplateConfig(app.Prompt));
            //var renderedPrompt = await promptTemplate.RenderAsync(_kernel);
            //Console.WriteLine(renderedPrompt);

            var func = _kernel.CreateFunctionFromPrompt(app.Prompt, settings);
            var chatResult = _kernel.InvokeStreamingAsync<StreamingTextContent>(function: func, arguments: new KernelArguments() { ["input"] = msg });
            int i = 0;

            await foreach (var content in chatResult)
            {
                result.choices[0].delta.content = content.Text.ConvertToString();
                string message = $"data: {JsonConvert.SerializeObject(result)}\n\n";
                await HttpContext.Response.WriteAsync(message, Encoding.UTF8);
                await HttpContext.Response.Body.FlushAsync();
                //模拟延迟。
                await Task.Delay(TimeSpan.FromMilliseconds(50));
            }

            await HttpContext.Response.WriteAsync("data: [DONE]");
            await HttpContext.Response.Body.FlushAsync();

            await HttpContext.Response.CompleteAsync();
        }

        /// <summary>
        /// 发送普通对话
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="msg"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        private async Task<string> SendChat( string msg, Apps app)
        {
            string result = "";
            if (string.IsNullOrEmpty(app.Prompt) || !app.Prompt.Contains("{{$input}}"))
            {
                //如果模板为空，给默认提示词
                app.Prompt = app.Prompt.ConvertToString() + "{{$input}}";
            }
            var _kernel = _kernelService.GetKernelByApp(app);
            var temperature = app.Temperature / 100;//存的是0~100需要缩小
            OpenAIPromptExecutionSettings settings = new() { Temperature = temperature };
            if (!string.IsNullOrEmpty(app.ApiFunctionList))
            {
                _kernelService.ImportFunctionsByApp(app, _kernel);
                settings = new() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions, Temperature = temperature };
            }
            var promptTemplateFactory = new KernelPromptTemplateFactory();
            var promptTemplate = promptTemplateFactory.Create(new PromptTemplateConfig(app.Prompt));

            var func = _kernel.CreateFunctionFromPrompt(app.Prompt, settings);
            var chatResult = await _kernel.InvokeAsync(function: func, arguments: new KernelArguments() { ["input"] = msg });
            if (chatResult.IsNotNull())
            {
                string answers = chatResult.GetValue<string>();
                result = answers;
            }
            return result;
        }

        /// <summary>
        /// 发送知识库问答
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="msg"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        private async Task<string> SendKms(string msg, Apps app)
        {
            var _kernel = _kernelService.GetKernelByApp(app);
            var _memory = _kMService.GetMemoryByKMS(app.KmsIdList.Split(",").FirstOrDefault());
            string result = "";
            //知识库问答
            var filters = new List<MemoryFilter>();

            var kmsidList = app.KmsIdList.Split(",");
            foreach (var kmsid in kmsidList)
            {
                filters.Add(new MemoryFilter().ByTag("kmsid", kmsid));
            }

            var xlresult = await _memory.SearchAsync(msg, index: "kms", filters: filters);
            string dataMsg = "";
            if (xlresult != null)
            {
                foreach (var item in xlresult.Results)
                {
                    foreach (var part in item.Partitions)
                    {
                        dataMsg += $"[file:{item.SourceName};Relevance:{(part.Relevance * 100).ToString("F2")}%]:{part.Text}{Environment.NewLine}";
                    }
                }
                KernelFunction jsonFun = _kernel.Plugins.GetFunction("KMSPlugin", "Ask");
                var chatResult = await _kernel.InvokeAsync(function: jsonFun,
                    arguments: new KernelArguments() { ["doc"] = dataMsg, ["history"] = "", ["questions"] = msg });
                if (chatResult.IsNotNull())
                {
                    string answers = chatResult.GetValue<string>();
                    result = answers;
                }  
            }
            return result;
        }
        /// <summary>
        /// 历史会话的会话总结
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private async Task<string> HistorySummarize(Apps app,OpenAIModel model)
        {
            var _kernel = _kernelService.GetKernelByApp(app);
            StringBuilder history = new StringBuilder();
            string questions = model.messages[model.messages.Count-1].content;
            for(int i=0;i<model.messages.Count()-1;i++)
            {
                var item = model.messages[i];
                history.Append($"{item.role}:{item.content}{Environment.NewLine}");
            }

            if (model.messages.Count() > 10)
            {
                //历史会话大于10条，进行总结
                var msg = await _kernelService.HistorySummarize(_kernel, questions, history.ToString());
                return msg;
            }
            else
            {
                var msg = $"history：{history.ToString()}{Environment.NewLine} user：{questions}"; ;
                return msg;
            }
        }
    }
}

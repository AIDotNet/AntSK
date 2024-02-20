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
        Kernel _kernel,
        MemoryServerless _memory
        ) : IOpenApiService
    {
        public async Task Chat(OpenAIModel model,string sk, HttpContext HttpContext)
        {
            OpenAIResult result = new OpenAIResult();
            result.created= DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            result.choices=new List<ChoicesModel>() { new ChoicesModel() { message=new OpenAIMessage() { role= "assistant" } } };
            Apps app = _apps_Repositories.GetFirst(p => p.SecretKey == sk);

            if (app.IsNotNull())
            {
                string msg= await HistorySummarize(model);
                switch (app.Type)
                {
                    case "chat":
                        //普通会话
                        if (model.stream)
                        {
                            await SendChatStream( HttpContext, result, app, msg);
                            return;
                        }
                        else 
                        {
                            result.choices[0].message.content = await SendChat(msg, app);
                        }
                        break;
                    case "kms":
                        //知识库问答
                        result.choices[0].message.content = await SendKms( msg, app);
                        break;
                }
            }
            await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(result));
            await HttpContext.Response.CompleteAsync();
        }

        private async Task SendChatStream( HttpContext HttpContext, OpenAIResult result, Apps app, string msg)
        {
            HttpContext.Response.Headers.Add("Content-Type", "text/event-stream");

            if (string.IsNullOrEmpty(app.Prompt))
            {
                //如果模板为空，给默认提示词
                app.Prompt = "{{$input}}";
            }
            var promptTemplateFactory = new KernelPromptTemplateFactory();
            var promptTemplate = promptTemplateFactory.Create(new PromptTemplateConfig(app.Prompt));
            var renderedPrompt = await promptTemplate.RenderAsync(_kernel);

            var func = _kernel.CreateFunctionFromPrompt(app.Prompt, new OpenAIPromptExecutionSettings());
            var chatResult = _kernel.InvokeStreamingAsync<StreamingChatMessageContent>(function: func, arguments: new KernelArguments() { ["input"] = msg });
            int i = 0;

            await foreach (var content in chatResult)
            {
                result.choices[0].message.content = content.Content.ConvertToString();
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
        /// 发送知识库问答
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="msg"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        private async Task<string> SendKms( string msg, Apps app)
        {
            string result = "";
            //知识库问答
            var filters = new List<MemoryFilter>();

            var kmsidList = app.KmsIdList.Split(",");
            foreach (var kmsid in kmsidList)
            {
                filters.Add(new MemoryFilter().ByTag("kmsid", kmsid));
            }

            var kmsResult = await _memory.AskAsync(msg, index: "kms", filters: filters);
            if (kmsResult != null)
            {
                if (!string.IsNullOrEmpty(kmsResult.Result))
                {
                    string answers = kmsResult.Result;
                    result = answers;     
                }
            }
            return result;
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
            if (string.IsNullOrEmpty(app.Prompt))
            {
                //如果模板为空，给默认提示词
                app.Prompt = "{{$input}}";
            }
            var promptTemplateFactory = new KernelPromptTemplateFactory();
            var promptTemplate = promptTemplateFactory.Create(new PromptTemplateConfig(app.Prompt));
            var renderedPrompt = await promptTemplate.RenderAsync(_kernel);

            var func = _kernel.CreateFunctionFromPrompt(app.Prompt, new OpenAIPromptExecutionSettings());
            var chatResult = await _kernel.InvokeAsync(function: func, arguments: new KernelArguments() { ["input"] = msg });
            if (chatResult.IsNotNull())
            {
                string answers = chatResult.GetValue<string>();
                result = answers;
            }
            return result;
        }

        /// <summary>
        /// 历史会话的会话总结
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private async Task<string> HistorySummarize(OpenAIModel model)
        {

            StringBuilder history = new StringBuilder();
            string questions = model.messages[model.messages.Count-1].content;
            for(int i=0;i<model.messages.Count()-1;i++)
            {
                var item = model.messages[i];
                history.Append($"{item.role}:{item.content}{Environment.NewLine}");
            }

            KernelFunction sunFun = _kernel.Plugins.GetFunction("ConversationSummaryPlugin", "SummarizeConversation");
            var summary = await _kernel.InvokeAsync(sunFun, new() { ["input"] = $"内容是：{history.ToString()} {Environment.NewLine} 请注意用中文总结" });
            string his = summary.GetValue<string>();
            var msg = $"历史对话:{his}{Environment.NewLine}用户问题：{Environment.NewLine}{questions}"; ;
            return msg;
        }
    }
}

using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Repositories;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System.Text;
using AntSK.Domain.Utils;
using AntSK.Domain.Domain.Model.Dto;
using AntSK.Domain.Domain.Model.Constant;
using DocumentFormat.OpenXml.Drawing;
using System.Reflection.Metadata;
using Microsoft.KernelMemory;
using System.Collections.Generic;
using Markdig;
using ChatHistory = Microsoft.SemanticKernel.ChatCompletion.ChatHistory;
using Microsoft.SemanticKernel.Plugins.Core;
using Azure.Core;
using AntSK.Domain.Domain.Model;

namespace AntSK.Domain.Domain.Service
{
    [ServiceDescription(typeof(IChatService), ServiceLifetime.Scoped)]
    public class ChatService(
        IKernelService _kernelService,
        IKMService _kMService,
        IKmsDetails_Repositories _kmsDetails_Repositories
        ) : IChatService
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="app"></param>
        /// <param name="questions"></param>
        /// <param name="history"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<StreamingKernelContent> SendChatByAppAsync(Apps app, string questions, ChatHistory history)
        {

            if (string.IsNullOrEmpty(app.Prompt) || !app.Prompt.Contains("{{$input}}"))
            {
                //如果模板为空，给默认提示词
                app.Prompt = app.Prompt.ConvertToString() + "{{$input}}";
            }
            KernelArguments args =new KernelArguments();
            if (history.Count > 10)
            {
                app.Prompt = @"${{ConversationSummaryPlugin.SummarizeConversation $history}}" + app.Prompt;
                args = new() {
                { "history", string.Join("\n", history.Select(x => x.Role + ": " + x.Content)) },
                { "input", questions }
                };
            }
            else 
            {
                args=new()
                {
                { "input", $"{string.Join("\n", history.Select(x => x.Role + ": " + x.Content))}{Environment.NewLine} user:{questions}" }
                };
            }
    
            var _kernel = _kernelService.GetKernelByApp(app);
            var temperature = app.Temperature / 100;//存的是0~100需要缩小
            OpenAIPromptExecutionSettings settings = new() { Temperature = temperature };
            if (!string.IsNullOrEmpty(app.ApiFunctionList) || !string.IsNullOrEmpty(app.NativeFunctionList))//这里还需要加上本地插件的
            {
                _kernelService.ImportFunctionsByApp(app, _kernel);
                settings.ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions;
            }
            var func = _kernel.CreateFunctionFromPrompt(app.Prompt, settings);
            var chatResult = _kernel.InvokeStreamingAsync(function: func, 
                arguments: args);
            await foreach (var content in chatResult)
            {
                yield return content;
            }
        }

        public async IAsyncEnumerable<StreamingKernelContent> SendKmsByAppAsync(Apps app, string questions, ChatHistory history, string filePath, List<RelevantSource> relevantSources = null)
        {
            var relevantSourceList = await _kMService.GetRelevantSourceList(app.KmsIdList, questions);
            var _kernel = _kernelService.GetKernelByApp(app);
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                var memory = _kMService.GetMemory(app);
                var fileId = Guid.NewGuid().ToString();
                var result = await memory.ImportDocumentAsync(new Microsoft.KernelMemory.Document(fileId).AddFile(filePath)
                          .AddTag(KmsConstantcs.KmsIdTag, app.Id)
                          , index: KmsConstantcs.KmsIndex);

                var filters = new MemoryFilter().ByTag(KmsConstantcs.KmsIdTag, app.Id);

                var searchResult = await memory.SearchAsync(questions, index: KmsConstantcs.KmsIndex, filters: [filters]);
                relevantSourceList.AddRange(searchResult.Results.SelectMany(item => item.Partitions.Select(part => new RelevantSource()
                {
                    SourceName = item.SourceName,
                    Text = Markdown.ToHtml(part.Text),
                    Relevance = part.Relevance
                })));
            }

            var dataMsg = new StringBuilder();
            if (relevantSourceList.Any())
            {
                relevantSources?.AddRange(relevantSourceList);
                foreach (var item in relevantSourceList)
                {
                    dataMsg.AppendLine(item.ToString());
                }

                KernelFunction jsonFun = _kernel.Plugins.GetFunction("KMSPlugin", "Ask1");
                var chatResult = _kernel.InvokeStreamingAsync(function: jsonFun,
                    arguments: new KernelArguments() { ["doc"] = dataMsg, ["history"] = string.Join("\n", history.Select(x => x.Role + ": " + x.Content)), ["questions"] = questions });

                await foreach (var content in chatResult)
                {
                    yield return content;
                }
            }
            else
            {
                yield return new StreamingTextContent(KmsConstantcs.KmsSearchNull);
            }
        }

        public async Task<ChatHistory> GetChatHistory(List<MessageInfo> MessageList)
        {
            ChatHistory history = new ChatHistory();
            if (MessageList.Count > 1)
            {

                foreach (var item in MessageList)
                {
                    if (item.IsSend)
                    {
                        history.AddUserMessage(item.Context);
                    }
                    else
                    {
                        history.AddAssistantMessage(item.Context);
                    }
                }
            }
            return history;
        }
    }
}
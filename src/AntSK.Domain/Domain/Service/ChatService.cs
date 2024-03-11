using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Repositories;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntSK.Domain.Utils;
using Microsoft.KernelMemory;
using AntSK.Domain.Model;
using MarkdownSharp;
using AntSK.Domain.Domain.Dto;

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
        public async IAsyncEnumerable<StreamingKernelContent> SendChatByAppAsync(Apps app, string questions, string history)
        {
            if (string.IsNullOrEmpty(app.Prompt) || !app.Prompt.Contains("{{$input}}"))
            {
                //如果模板为空，给默认提示词
                app.Prompt = app.Prompt.ConvertToString() + "{{$input}}";
            }
            var _kernel = _kernelService.GetKernelByApp(app);
            var temperature = app.Temperature / 100;//存的是0~100需要缩小
            OpenAIPromptExecutionSettings settings = new() { Temperature = temperature };
            if (!string.IsNullOrEmpty(app.ApiFunctionList))//这里还需要加上本地插件的
            {
                _kernelService.ImportFunctionsByApp(app, _kernel);
                settings.ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions;
            }
            var func = _kernel.CreateFunctionFromPrompt(app.Prompt, settings);
            var chatResult = _kernel.InvokeStreamingAsync(function: func, arguments: new KernelArguments() { ["input"] = $"{history}{Environment.NewLine} user:{questions}" });
            await foreach (var content in chatResult)
            {
                yield return content;
            }
        }

        public async IAsyncEnumerable<StreamingKernelContent> SendKmsByAppAsync(Apps app, string questions, string history, List<RelevantSource> relevantSources = null)
        {
            var _kernel = _kernelService.GetKernelByApp(app);
            //知识库问答
            var filters = new List<MemoryFilter>();
            var kmsidList = app.KmsIdList.Split(",");
            //只取第一个知识库的配置
            var _memory = _kMService.GetMemoryByKMS(kmsidList.FirstOrDefault());
            foreach (var kmsid in kmsidList)
            {
                filters.Add(new MemoryFilter().ByTag("kmsid", kmsid));
            }
            var xlresult = await _memory.SearchAsync(questions, index: "kms", filters: filters);
            string dataMsg = "";
            if (xlresult != null)
            {
                foreach (var item in xlresult.Results)
                {
                    foreach (var part in item.Partitions)
                    {
                        dataMsg += $"[file:{item.SourceName};Relevance:{(part.Relevance * 100).ToString("F2")}%]:{part.Text}{Environment.NewLine}";

                        if (relevantSources.IsNotNull())
                        {
                            var markdown = new Markdown();
                            string sourceName = item.SourceName;
                            var fileDetail = _kmsDetails_Repositories.GetFirst(p => p.FileGuidName == item.SourceName);
                            if (fileDetail.IsNotNull())
                            {
                                sourceName = fileDetail.FileName;
                            }
                            relevantSources.Add(new RelevantSource() { SourceName = sourceName, Text = markdown.Transform(part.Text), Relevance = part.Relevance });
                        }
                    }
                }
                KernelFunction jsonFun = _kernel.Plugins.GetFunction("KMSPlugin", "Ask");
                var chatResult = _kernel.InvokeStreamingAsync(function: jsonFun,
                    arguments: new KernelArguments() { ["doc"] = dataMsg, ["history"] = history, ["questions"] = questions });

                MessageInfo info = null;
                var markdown1 = new Markdown();
                await foreach (var content in chatResult)
                {
                    yield return content;
                }
            }
        }
    }
}
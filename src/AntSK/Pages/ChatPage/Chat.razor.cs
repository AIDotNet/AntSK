using AntDesign;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Model;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using Azure.AI.OpenAI;
using Azure.Core;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using MarkdownSharp;
using Microsoft.AspNetCore.Components;
using Microsoft.KernelMemory;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;
using RestSharp;
using SqlSugar;
using System;
using System.Text;

namespace AntSK.Pages.ChatPage
{
    public partial class Chat
    {
        [Parameter]
        public string AppId { get; set; }
        [Inject]
        protected MessageService? Message { get; set; }
        [Inject]
        protected IApps_Repositories _apps_Repositories { get; set; }
        [Inject]
        protected IApis_Repositories _apis_Repositories { get; set; }
        [Inject]
        protected IKmss_Repositories _kmss_Repositories { get; set; }
        [Inject]
        protected IKmsDetails_Repositories _kmsDetails_Repositories { get; set; }

        //[Inject]
        //protected Kernel _kernel { get; set; }
        [Inject]
        protected IKernelService _kernelService { get; set; }
        [Inject]
        protected IKMService _kMService { get; set; }

        protected bool _loading = false;
        protected List<MessageInfo> MessageList = [];
        protected string? _messageInput;
        protected string _json = "";
        protected bool Sendding = false;

        List<RelevantSource> RelevantSources = new List<RelevantSource>();

        protected List<Apps> _list = new List<Apps>();
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _list = _apps_Repositories.GetList();
        }
        protected async Task OnSendAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_messageInput))
                {
                    _ = Message.Info("请输入消息", 2);
                    return;
                }

                if (string.IsNullOrWhiteSpace(AppId))
                {
                    _ = Message.Info("请选择应用进行测试", 2);
                    return;
                }

                MessageList.Add(new MessageInfo()
                {
                    ID = Guid.NewGuid().ToString(),
                    Context = _messageInput,
                    CreateTime = DateTime.Now,
                    IsSend = true
                });


                Sendding = true;
                await SendAsync(_messageInput);
                _messageInput = "";
                Sendding = false;
            }
            catch (System.Exception ex)
            {
                Sendding = false;
                _ = Message.Error("异常:"+ex.Message, 2);
            }
        }
        protected async Task OnCopyAsync(MessageInfo item)
        {
            await Task.Run(() =>
            {
                _messageInput = item.Context;
            });
        }

        protected async Task OnClearAsync(string id)
        {
            await Task.Run(() =>
            {
                MessageList = MessageList.Where(w => w.ID != id).ToList();
            });
        }

        protected async Task<bool> SendAsync(string questions)
        {
            string msg = "";
            //处理多轮会话
            Apps app = _apps_Repositories.GetFirst(p => p.Id == AppId);
            if (MessageList.Count > 0)
            {
                msg = await HistorySummarize(app,questions);
            }

            switch (app.Type)
            {
                case "chat":
                    //普通会话
                    await SendChat(questions, msg, app);
                    break;
                case "kms":
                    //知识库问答
                    await SendKms(questions, msg, app);
                    break;
            }

            return await Task.FromResult(true);
        }

        /// <summary>
        /// 发送知识库问答
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="msg"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        private async Task SendKms(string questions, string msg, Apps app)
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
            var  xlresult = await _memory.SearchAsync(questions, index: "kms", filters: filters);
            string dataMsg = "";
            if (xlresult != null)
            {
                foreach (var item in xlresult.Results)
                {
                    foreach (var part in item.Partitions)
                    {
                        dataMsg += $"[file:{item.SourceName};Relevance:{(part.Relevance*100).ToString("F2")}%]:{part.Text}{Environment.NewLine}";
                        //输出调试信息
                        var markdown = new Markdown();
                        string sourceName = item.SourceName;
                        var fileDetail = _kmsDetails_Repositories.GetFirst(p => p.FileGuidName == item.SourceName);
                        if (fileDetail.IsNotNull())
                        {
                            sourceName = fileDetail.FileName;
                        }
                        RelevantSources.Add(new RelevantSource() { SourceName = sourceName, Text = markdown.Transform(part.Text), Relevance = part.Relevance });
                    }
                }
              
                KernelFunction jsonFun = _kernel.Plugins.GetFunction("KMSPlugin", "Ask");
                var chatResult = _kernel.InvokeStreamingAsync<StreamingChatMessageContent>(function: jsonFun, 
                    arguments: new KernelArguments() { ["doc"] = dataMsg, ["history"] = msg, ["questions"]=questions });

                MessageInfo info = null;
                var markdown1 = new Markdown();
                await foreach (var content in chatResult)
                {
                    if (info == null)
                    {
                        info = new MessageInfo();
                        info.ID = Guid.NewGuid().ToString();
                        info.Context = content?.Content?.ConvertToString();
                        info.HtmlAnswers = content?.Content?.ConvertToString();
                        info.CreateTime = DateTime.Now;

                        MessageList.Add(info);
                    }
                    else
                    {
                        info.HtmlAnswers += content.Content;
                        await Task.Delay(50);
                    }
                    await InvokeAsync(StateHasChanged);
                }
                //全部处理完后再处理一次Markdown
                if (info.IsNotNull())
                {
                    info!.HtmlAnswers = markdown1.Transform(info.HtmlAnswers);
                }

                await InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// 发送普通对话
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="msg"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        private async Task SendChat(string questions, string msg, Apps app)
        {
            var _kernel = _kernelService.GetKernelByApp(app);
            if (string.IsNullOrEmpty(app.Prompt)||!app.Prompt.Contains("{{$input}}"))
            {
                //如果模板为空，给默认提示词
                app.Prompt = app.Prompt.ConvertToString()+"{{$input}}";
            }
            var temperature = app.Temperature/100;//存的是0~100需要缩小
            OpenAIPromptExecutionSettings settings = new() {Temperature= temperature };
            if (!string.IsNullOrEmpty(app.ApiFunctionList))
            {
                _kernelService.ImportFunctionsByApp(app, _kernel);
                settings = new() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,Temperature = temperature };
            }
            
            var func = _kernel.CreateFunctionFromPrompt(app.Prompt, settings);
            var chatResult = _kernel.InvokeStreamingAsync<StreamingChatMessageContent>(function: func, arguments: new KernelArguments() { ["input"] = msg });
            MessageInfo info = null;
            var markdown = new Markdown();
            await foreach (var content in chatResult)
            {
                if (info == null)
                {
                    info = new MessageInfo();
                    info.ID = Guid.NewGuid().ToString();
                    info.Context = content?.Content?.ConvertToString();
                    info.HtmlAnswers = content?.Content?.ConvertToString();
                    info.CreateTime = DateTime.Now;

                    MessageList.Add(info);
                }
                else
                {
                    info.HtmlAnswers += content.Content;
                    await Task.Delay(50); 
                }
                await InvokeAsync(StateHasChanged);
            }
            //全部处理完后再处理一次Markdown
            if (info.IsNotNull())
            {
                info!.HtmlAnswers = markdown.Transform(info.HtmlAnswers);
            }
     
            await InvokeAsync(StateHasChanged);
        }

       

        /// <summary>
        /// 历史会话的会话总结
        /// </summary>
        /// <param name="questions"></param>
        /// <returns></returns>
        private async Task<string> HistorySummarize(Apps app,string questions)
        {
            var _kernel = _kernelService.GetKernelByApp(app);
            if (MessageList.Count > 1)
            {
                StringBuilder history = new StringBuilder();
                foreach (var item in MessageList)
                {
                    if (item.IsSend)
                    {
                        history.Append($"user:{item.Context}{Environment.NewLine}");
                    }
                    else
                    {
                        history.Append($"assistant:{item.Context}{Environment.NewLine}");
                    }
                }
                if (MessageList.Count > 10)
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
            else 
            {
                return questions;
            }
        }
    }

    public class RelevantSource
    {
        public string SourceName { get; set; }

        public string Text { get; set; }
        public float Relevance { get; set; }
    }
}

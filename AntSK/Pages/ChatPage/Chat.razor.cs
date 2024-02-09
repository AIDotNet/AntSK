using AntDesign;
using AntSK.Domain.Model;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using Azure.AI.OpenAI;
using Azure.Core;
using MarkdownSharp;
using Microsoft.AspNetCore.Components;
using Microsoft.KernelMemory;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;
using SqlSugar;
using System;

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
        protected IKmss_Repositories _kmss_Repositories { get; set; }
        [Inject]
        protected IKmsDetails_Repositories _kmsDetails_Repositories { get; set; }
        [Inject]
        protected MemoryServerless _memory { get; set; }
        [Inject]
        protected Kernel _kernel { get; set; }

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
            _list= _apps_Repositories.GetList();
        }
        protected async Task OnSendAsync()
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

            Sendding = true;
            await SendAsync(_messageInput);
            _messageInput = "";
            Sendding = false;

        }
        protected async Task OnCopyAsync(MessageInfo item)
        {
            await Task.Run(() =>
            {
                _messageInput = item.Questions;
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
            Apps app=_apps_Repositories.GetFirst(p => p.Id == AppId);
            switch (app.Type)
            {
                case "chat":
                    {
                        //普通会话
                        var promptTemplateFactory = new KernelPromptTemplateFactory();
                        var promptTemplate = promptTemplateFactory.Create(new PromptTemplateConfig(app.Prompt));
                        var renderedPrompt = await promptTemplate.RenderAsync(_kernel);

                        var func = _kernel.CreateFunctionFromPrompt(app.Prompt, new OpenAIPromptExecutionSettings());
                        var chatResult = _kernel.InvokeStreamingAsync<StreamingChatMessageContent>(function: func, arguments: new KernelArguments() { ["input"] = questions });
                        MessageInfo info = null;
                        var markdown = new Markdown();
                        await foreach (var content in chatResult)
                        {
                            if (info == null)
                            {
                                info = new MessageInfo();
                                info.ID = Guid.NewGuid().ToString();
                                info.Questions = questions;
                                info.Answers = content.Content!;
                                info.HtmlAnswers = content.Content!;
                                info.CreateTime = DateTime.Now;

                                MessageList.Add(info);
                            }
                            else
                            {
                                info.HtmlAnswers += content.Content;
                            }
                            await InvokeAsync(StateHasChanged);
                        }
                        //全部处理完后再处理一次Markdown
                        info!.HtmlAnswers = markdown.Transform(info.HtmlAnswers);
                        await InvokeAsync(StateHasChanged);
                    }
                    break;
                case "kms":
                    {
                        //知识库问答
                        var filters = new List<MemoryFilter>();

                        var kmsidList = app.KmsIdList.Split(",");
                        foreach (var kmsid in kmsidList)
                        {
                            filters.Add(new MemoryFilter().ByTag("kmsid", kmsid));
                        }

                        var kmsResult = await _memory.AskAsync(questions, index: "kms", filters: filters);
                        if (kmsResult != null)
                        {
                            if (!string.IsNullOrEmpty(kmsResult.Result))
                            {
                                string answers = kmsResult.Result;
                                var markdown = new Markdown();
                                string htmlAnswers = markdown.Transform(answers);
                                var info1 = new MessageInfo()
                                {
                                    ID = Guid.NewGuid().ToString(),
                                    Questions = questions,
                                    Answers = answers,
                                    HtmlAnswers = htmlAnswers,
                                    CreateTime = DateTime.Now,
                                };
                                MessageList.Add(info1);
                            }

                            foreach (var x in kmsResult.RelevantSources)
                            {
                                foreach (var xsd in x.Partitions)
                                {
                                    string sourceName = x.SourceName;
                                    var fileDetail = _kmsDetails_Repositories.GetFirst(p => p.FileGuidName == x.SourceName);
                                    if (fileDetail.IsNotNull())
                                    {
                                        sourceName = fileDetail.FileName;
                                    }
                                    RelevantSources.Add(new RelevantSource() { SourceName = sourceName, Text = xsd.Text, Relevance = xsd.Relevance });
                                }
                            }
                        }
                    }
                    break;
            }

            return await Task.FromResult(true);
        }
    }

    public class RelevantSource
    {
        public string SourceName { get; set; }

        public string Text { get; set; }
        public float Relevance { get; set; }
    }
}

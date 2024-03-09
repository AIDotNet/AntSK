using AntDesign;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Model;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using MarkdownSharp;
using Microsoft.AspNetCore.Components;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SqlSugar;
using System.Text;

namespace AntSK.Pages.ChatPage
{
    public partial class OpenChat
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
        protected IKernelService _kernelService { get; set; }
        [Inject]
        protected IKMService _kMService { get; set; }
        [Inject]
        IConfirmService _confirmService { get; set; }
        [Inject]
        IChatService _chatService { get; set; }

        protected bool _loading = false;
        protected List<MessageInfo> MessageList = [];
        protected string? _messageInput;
        protected string _json = "";
        protected bool Sendding = false;

        protected Apps app = new Apps();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            app = _apps_Repositories.GetFirst(p => p.Id == AppId);
        }

        protected async Task OnClearAsync()
        {
            if (MessageList.Count > 0)
            {
                var content = "是否要清理会话记录";
                var title = "清理";
                var result = await _confirmService.Show(content, title, ConfirmButtons.YesNo);
                if (result == ConfirmResult.Yes)
                {
                    MessageList.Clear();
                    _ = Message.Info("清理成功");
                }
            }
            else
            {
                _ = Message.Info("没有会话记录");
            }
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
                Console.WriteLine("异常:" + ex.Message);
                _ = Message.Error("异常:" + ex.Message, 2);
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
                msg = await HistorySummarize(app, questions);
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
            var _memory = _kMService.GetMemoryByKMS(app.KmsIdList.Split(",").FirstOrDefault());
            //知识库问答
            var filters = new List<MemoryFilter>();

            var kmsidList = app.KmsIdList.Split(",");
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
                    }
                }
                KernelFunction jsonFun = _kernel.Plugins.GetFunction("KMSPlugin", "Ask");
                var chatResult = _kernel.InvokeStreamingAsync(function: jsonFun,
                    arguments: new KernelArguments() { ["doc"] = dataMsg, ["history"] = msg, ["questions"] = questions });

                MessageInfo info = null;
                var markdown1 = new Markdown();
                await foreach (var content in chatResult)
                {
                    if (info == null)
                    {
                        info = new MessageInfo();
                        info.ID = Guid.NewGuid().ToString();
                        info.Context = content.ConvertToString();
                        info.HtmlAnswers = content.ConvertToString();
                        info.CreateTime = DateTime.Now;

                        MessageList.Add(info);
                    }
                    else
                    {
                        info.HtmlAnswers += content.ConvertToString();
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
        /// <param name="history"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        private async Task SendChat(string questions, string history, Apps app)
        {
            MessageInfo info = null;
            var markdown = new Markdown();
            var chatResult = _chatService.ChatByAppAsync(app, questions, history);
            await foreach (var content in chatResult)
            {
                if (info == null)
                {
                    info = new MessageInfo();
                    info.ID = Guid.NewGuid().ToString();
                    info.Context = content.ConvertToString();
                    info.HtmlAnswers = content.ConvertToString();
                    info.CreateTime = DateTime.Now;

                    MessageList.Add(info);
                }
                else
                {
                    info.HtmlAnswers += content.ConvertToString();
                    await Task.Delay(50);
                }
                await InvokeAsync(StateHasChanged);
            }
            //全部处理完后再处理一次Markdown
            info!.HtmlAnswers = markdown.Transform(info.HtmlAnswers);
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// 历史会话的会话总结
        /// </summary>
        /// <param name="questions"></param>
        /// <returns></returns>
        private async Task<string> HistorySummarize(Apps app, string questions)
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
                return "";
            }
        }
    }
}

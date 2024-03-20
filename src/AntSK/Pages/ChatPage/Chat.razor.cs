using AntDesign;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;
using Markdig;
using AntSK.Domain.Domain.Model;
using AntSK.Domain.Domain.Model.Dto;

namespace AntSK.Pages.ChatPage
{
    public partial class Chat
    {
        [Parameter] public string AppId { get; set; }
        [Inject] protected MessageService? Message { get; set; }
        [Inject] protected IApps_Repositories _apps_Repositories { get; set; }
        [Inject] protected IApis_Repositories _apis_Repositories { get; set; }
        [Inject] protected IKmss_Repositories _kmss_Repositories { get; set; }
        [Inject] protected IKmsDetails_Repositories _kmsDetails_Repositories { get; set; }
        [Inject] private IJSRuntime _JSRuntime { get; set; }

        [Inject] protected IKernelService _kernelService { get; set; }
        [Inject] protected IKMService _kMService { get; set; }
        [Inject] private IConfirmService _confirmService { get; set; }
        [Inject] private IChatService _chatService { get; set; }

        [Inject] private ILogger<Chat> Logger { get; set; }

        protected bool _loading = false;
        protected List<MessageInfo> MessageList = [];
        protected string? _messageInput;
        protected string _json = "";
        protected bool Sendding = false;

        private List<RelevantSource> _relevantSources = new List<RelevantSource>();

        protected List<Apps> _list = new List<Apps>();

        private List<UploadFileItem> fileList = [];

        private Upload _uploadRef;

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

                var filePath = fileList.FirstOrDefault()?.Url;
                var fileName = fileList.FirstOrDefault()?.FileName;

                MessageList.Add(new MessageInfo()
                {
                    ID = Guid.NewGuid().ToString(),
                    Context = _messageInput,
                    CreateTime = DateTime.Now,
                    IsSend = true,
                    FilePath = filePath,
                    FileName = fileName
                });

                var prompt = _messageInput;
                _messageInput = "";
                fileList.Clear();

                Sendding = true;
        
                await SendAsync(prompt, filePath);

                Sendding = false;
            }
            catch (System.Exception ex)
            {
                Sendding = false;
                Logger.LogError(ex, "对话异常");
                _ = Message.Error("异常:" + ex.Message, 2);
            }
        }

        protected async Task OnCopyAsync(MessageInfo item)
        {
            await Task.Run(() => { _messageInput = item.Context; });
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

        protected async Task<bool> SendAsync(string questions, string? filePath)
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
                case "chat" when filePath == null:
                    //普通会话
                    await SendChat(questions, msg, app);
                    break;

                default:
                    //知识库问答
                    await SendKms(questions, msg, filePath, app);
                    break;
            }

            return await Task.FromResult(true);
        }

        /// <summary>
        /// 发送知识库问答
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="msg"></param>
        /// <param name="filePath"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        private async Task SendKms(string questions, string msg, string filePath, Apps app)
        {
            MessageInfo info = null;
            var chatResult = _chatService.SendKmsByAppAsync(app, questions, msg, filePath, _relevantSources);
            await foreach (var content in chatResult)
            {
                if (info == null)
                {
                    info = new MessageInfo();
                    info.ID = Guid.NewGuid().ToString();
                    info.Context = content?.ConvertToString();
                    info.HtmlAnswers = content?.ConvertToString();
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
            await MarkDown(info);
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
            var chatResult = _chatService.SendChatByAppAsync(app, questions, history);
            await foreach (var content in chatResult)
            {
                if (info == null)
                {
                    info = new MessageInfo();
                    info.ID = Guid.NewGuid().ToString();
                    info.Context = content?.ConvertToString();
                    info.HtmlAnswers = content?.ConvertToString();
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
            await MarkDown(info);
        }

        private async Task MarkDown(MessageInfo info)
        {
            if (info.IsNotNull())
            {
                // info!.HtmlAnswers = markdown.Transform(info.HtmlAnswers);
                info!.HtmlAnswers = Markdown.ToHtml(info.HtmlAnswers);
            }

            await InvokeAsync(StateHasChanged);
            await _JSRuntime.InvokeVoidAsync("Prism.highlightAll");
            await _JSRuntime.ScrollToBottomAsync("scrollDiv");
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
                    var msg =
                        $"history：{Environment.NewLine}{history.ToString()}{Environment.NewLine}{Environment.NewLine}";
                    return msg;
                }
            }
            else
            {
                return "";
            }
        }

        private void OnSingleCompleted(UploadInfo fileInfo)
        {
            fileList.Add(new()
            {
                FileName = fileInfo.File.FileName,
                Url = fileInfo.File.Url = fileInfo.File.Response,
                Ext = fileInfo.File.Ext,
                State = UploadState.Success,
            });
            _kMService.OnSingleCompleted(fileInfo);
        }

        private async Task<bool> HandleFileRemove(UploadFileItem file)
        {
            fileList.RemoveAll(x => x.FileName == file.FileName);
            await Task.Yield();
            return true;
        }
    }
}
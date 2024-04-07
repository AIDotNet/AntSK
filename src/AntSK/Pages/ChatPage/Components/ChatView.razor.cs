using AntDesign;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Domain.Model;
using AntSK.Domain.Domain.Model.Dto;
using AntSK.Domain.Domain.Model.Enum;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using AntSK.LLM.StableDiffusion;
using Blazored.LocalStorage;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;

namespace AntSK.Pages.ChatPage.Components
{
    public partial class ChatView
    {
        [Parameter]
        public string AppId { get; set; }

        [Parameter]
        public bool ShowTitle { get; set; } = false;
        [Parameter]
        public EventCallback<List<RelevantSource>> OnRelevantSources { get; set; }
        [Inject] protected MessageService? Message { get; set; }
        [Inject] protected IApps_Repositories _apps_Repositories { get; set; }
        [Inject] protected IKmss_Repositories _kmss_Repositories { get; set; }
        [Inject] protected IKmsDetails_Repositories _kmsDetails_Repositories { get; set; }
        [Inject] protected IKernelService _kernelService { get; set; }
        [Inject] protected IKMService _kMService { get; set; }
        [Inject] IConfirmService _confirmService { get; set; }
        [Inject] IChatService _chatService { get; set; }
        [Inject] IJSRuntime _JSRuntime { get; set; }
        [Inject] ILocalStorageService _localStorage { get; set; }

        protected List<MessageInfo> MessageList = [];
        protected string? _messageInput;
        protected string _json = "";
        protected bool Sendding = false;

        protected Apps app = new Apps();

        private List<UploadFileItem> fileList = [];

        private List<RelevantSource> _relevantSources = new List<RelevantSource>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            LoadData();
            var msgs = await _localStorage.GetItemAsync<List<MessageInfo>>("msgs");
            if (msgs != null && msgs.Count > 0)
            {
                MessageList = msgs;
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            LoadData();
        }

        private void LoadData()
        {
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
                    await _localStorage.SetItemAsync<List<MessageInfo>>("msgs", MessageList);
                    await InvokeAsync(StateHasChanged);
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
                var filePath = fileList.FirstOrDefault()?.Url;
                var fileName = fileList.FirstOrDefault()?.FileName;

                MessageList.Add(new MessageInfo()
                {
                    ID = Guid.NewGuid().ToString(),
                    Context = _messageInput,
                    CreateTime = DateTime.Now,
                    IsSend = true
                });
       
                Sendding = true;
                await SendAsync(_messageInput,filePath);
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

        protected async Task<bool> SendAsync(string questions, string? filePath)
        {
            ChatHistory history = new ChatHistory();
            //处理多轮会话
            Apps app = _apps_Repositories.GetFirst(p => p.Id == AppId);
            if (MessageList.Count > 0)
            {
                history = await _chatService.GetChatHistory(MessageList);
            }

            if (app.Type == AppType.chat.ToString() && (filePath == null || app.EmbeddingModelID.IsNull()))
            {
                await SendChat(questions, history, app);
            }
            else if (app.Type == AppType.kms.ToString() || filePath != null || app.EmbeddingModelID.IsNotNull())
            {
                await SendKms(questions, history, app, filePath);
               
            }
            else if (app.Type == AppType.img.ToString())
            {
                await SendImg(questions,app);
            }

            //缓存消息记录
            if (app.Type != AppType.img.ToString())
            {
                await _localStorage.SetItemAsync<List<MessageInfo>>("msgs", MessageList);
            }


            return await Task.FromResult(true);
        }

        private async Task SendImg(string questions,Apps app)
        {
            MessageInfo info = new MessageInfo();
            info.ID = Guid.NewGuid().ToString();
            info.CreateTime = DateTime.Now;
            var base64= await _chatService.SendImgByAppAsync(app, questions);
            if (string.IsNullOrEmpty(base64))
            {
                info.Context = "生成失败";
            }
            else 
            {
                info.Context = $"<img src=\"data:image/jpeg;base64,{base64}\" alt=\"Base64 Image\" />";
            }
            MessageList.Add(info);
        }

        /// <summary>
        /// 发送知识库问答
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="msg"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        private async Task SendKms(string questions, ChatHistory history, Apps app, string? filePath)
        {
            MessageInfo info = null;
            var chatResult = _chatService.SendKmsByAppAsync(app, questions, history, filePath, _relevantSources);
            await foreach (var content in chatResult)
            {
                if (info == null)
                {
                    info = new MessageInfo();
                    info.ID = Guid.NewGuid().ToString();
                    info.Context = content.ConvertToString();
                    info.CreateTime = DateTime.Now;

                    MessageList.Add(info);
                }
                else
                {
                    info.Context += content.ConvertToString();
                    await Task.Delay(50);
                }
                await InvokeAsync(StateHasChanged);
            }
            await OnRelevantSources.InvokeAsync(_relevantSources);
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
        private async Task SendChat(string questions, ChatHistory history, Apps app)
        {
            MessageInfo info = null;
            var chatResult = _chatService.SendChatByAppAsync(app, questions, history);
            await foreach (var content in chatResult)
            {
                if (info == null)
                {
                    info = new MessageInfo();
                    info.ID = Guid.NewGuid().ToString();
                    info.Context = content.ConvertToString();
                    info.CreateTime = DateTime.Now;

                    MessageList.Add(info);
                }
                else
                {
                    info.Context += content.ConvertToString();
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
                info!.Context = Markdown.ToHtml(info.Context);

            }
            await InvokeAsync(StateHasChanged);
            await _JSRuntime.InvokeVoidAsync("Prism.highlightAll");
            await _JSRuntime.ScrollToBottomAsync("scrollDiv");
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

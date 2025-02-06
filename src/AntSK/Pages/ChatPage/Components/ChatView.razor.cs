using AntDesign;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Domain.Model;
using AntSK.Domain.Domain.Model.Dto;
using AntSK.Domain.Domain.Model.Enum;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using AntSK.LLM.StableDiffusion;
using AntSK.Models;
using AntSK.Pages.KmsPage;
using Blazored.LocalStorage;
using DocumentFormat.OpenXml.InkML;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

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
        [Inject] IChats_Repositories _chats_Repositories { get; set; }
        [Inject] ProtectedSessionStorage _protectedSessionStore { get; set; }

        [Inject] protected ILogger<ChatView> _logger { get; set; }

        protected List<Chats> MessageList = [];
        protected string? _messageInput;
        protected string _json = "";
        protected bool Sendding = false;

        protected Apps app = new Apps();

        private List<UploadFileItem> fileList = [];

        private List<RelevantSource> _relevantSources = new List<RelevantSource>();

        private string _userName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadData();
          
        }

        protected override async Task OnParametersSetAsync()
        {
            await LoadData();
        }

        /// <summary>
        /// 初始化加载数据
        /// </summary>
        /// <returns></returns>
        private async Task LoadData()
        {
            app = _apps_Repositories.GetFirst(p => p.Id == AppId);
            var userSessionStorageResult = await _protectedSessionStore.GetAsync<UserSession>("UserSession");
            var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;
            _userName = userSession?.UserName;
            await GetMsgList();
            await MarkDown();
        }
        /// <summary>
        /// 获取聊天记录列表
        /// </summary>
        /// <returns></returns>
        private async Task GetMsgList()
        {
            List<Chats> msgs = new List<Chats>();
            if (string.IsNullOrEmpty(_userName))
            {
                //匿名访问使用localstore
                 msgs = await _localStorage.GetItemAsync<List<Chats>>($"msgs:{AppId}");
            }
            else 
            {
                msgs = await _chats_Repositories.GetListAsync(p => p.AppId == AppId && p.UserName == _userName);
            }
            if (msgs != null && msgs.Count > 0)
            {
                MessageList = msgs;
            }
        }

        /// <summary>
        /// 清空聊天记录列表
        /// </summary>
        /// <returns></returns>
        private async Task ClearMsgList() 
        {
            MessageList.Clear();
            if (string.IsNullOrEmpty(_userName))
            {
                await _localStorage.SetItemAsync<List<Chats>>($"msgs:{AppId}", MessageList);
            }
            else 
            {
                await _chats_Repositories.DeleteAsync(p => p.AppId == AppId && p.UserName == _userName);
            }        
        }
        /// <summary>
        /// 保存聊天记录
        /// </summary>
        /// <returns></returns>
        private async Task SaveMsg(List<Chats> MessageList)
        {
            if (string.IsNullOrEmpty(_userName))
            {
                await _localStorage.SetItemAsync<List<Chats>>($"msgs:{AppId}", MessageList);
            }
            else 
            {
                if (MessageList.Count() > 0)
                {
                    await _chats_Repositories.InsertAsync(MessageList.LastOrDefault());
                }
            }          
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

                    await ClearMsgList();
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

                var chat = new Chats()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = _userName,
                    AppId = AppId,
                    Context = _messageInput,
                    CreateTime = DateTime.Now,
                    IsSend = true
                };
                MessageList.Add(chat);
                if (!string.IsNullOrEmpty(_userName))
                {
                    await _chats_Repositories.InsertAsync(chat);
                }

                Sendding = true;
                Task.Run(async () =>
                {
                    await SendAsync(_messageInput, filePath);
                }).ContinueWith(task => {

                    _messageInput = "";
                    Sendding = false;
                    InvokeAsync(StateHasChanged);
                });


            }
            catch (System.Exception ex)
            {
                Sendding = false;
                _logger.LogError("异常:" + ex.Message);
                _ = Message.Error("异常:" + ex.Message, 2);
            }

        }


        protected async Task OnCopyAsync(Chats item)
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
                MessageList = MessageList.Where(w => w.Id != id).ToList();
            });
        }

        /// <summary>
        /// 开始发送消息
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected async Task<bool> SendAsync(string questions, string? filePath)
        {

            //处理多轮会话
            Apps app = _apps_Repositories.GetFirst(p => p.Id == AppId);
            ChatHistory history = new ChatHistory();

            if (app.Type == AppType.chat.ToString() && (filePath == null || app.EmbeddingModelID.IsNull()))
            {
                if (!string.IsNullOrEmpty(app.Prompt))
                {
                    history = new ChatHistory(app.Prompt.ConvertToString());
                }
                //聊天应用增加系统角色
                if (MessageList.Count > 0)
                {
                    history = await _chatService.GetChatHistory(MessageList, history);
                }
                await SendChat(history, app);
            }
            else if (app.Type == AppType.kms.ToString() || filePath != null || app.EmbeddingModelID.IsNotNull())
            {
                if (MessageList.Count > 0)
                {
                    history = await _chatService.GetChatHistory(MessageList, history);
                }
                await SendKms(questions, history, app, filePath);

            }
            else if (app.Type == AppType.img.ToString())
            {
                await SendImg(questions, app);
            }

            //缓存消息记录
            if (app.Type != AppType.img.ToString())
            {
                await SaveMsg(MessageList);
                if (OnRelevantSources.IsNotNull())
                {
                    await OnRelevantSources.InvokeAsync(_relevantSources);
                }
            }
            return true;
        }

        /// <summary>
        /// 发送图片对话
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        private async Task SendImg(string questions,Apps app)
        {
            Chats info = new Chats();
            info.Id = Guid.NewGuid().ToString();
            info.UserName=_userName;
            info.AppId=AppId;
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
            Chats info = new Chats()
            {
                Id = Guid.NewGuid().ToString(),
                AppId = AppId,
                UserName = _userName,
                CreateTime = DateTime.Now,
                Context=""
            };
            MessageList.Add(info);
            var chatResult = _chatService.SendKmsByAppAsync(app, questions, history, filePath, _relevantSources);
            StringBuilder rawContent = new StringBuilder();
            await foreach (var content in chatResult)
            {
                rawContent.Append(content.ConvertToString());
                info.Context = Markdown.ToHtml(rawContent.ToString().Replace("<think>", "<div class=\"think\">").Replace("</think>", "</div>"));
                await Task.Delay(30);
                await InvokeAsync(StateHasChanged);
            }
            //全部处理完后再处理一次Markdown 处理代码高亮
            await MarkDown();
        }

        /// <summary>
        /// 发送普通对话
        /// </summary>
        /// <param name="history"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        private async Task SendChat(ChatHistory history, Apps app)
        {
            Chats info = null;
            var chatResult = _chatService.SendChatByAppAsync(app, history);
            StringBuilder rawContent = new StringBuilder();
            await foreach (var content in chatResult)
            {
                if (info == null)
                {
                    rawContent.Append(content.ConvertToString());
                    info = new Chats();
                    info.Id = Guid.NewGuid().ToString();
                    info.UserName = _userName;
                    info.AppId = AppId;
                    info.Context = content.ConvertToString();
                    info.CreateTime = DateTime.Now;

                    MessageList.Add(info);
                }
                else
                {
                    rawContent.Append(content.ConvertToString());
                }
                info.Context = Markdown.ToHtml(rawContent.ToString().Replace("<think>", "<div class=\"think\">").Replace("</think>", "</div>"));
                await Task.Delay(30);
                await InvokeAsync(StateHasChanged);
            }
            //全部处理完后再处理一次Markdown 处理代码高亮
            await MarkDown();
        }

        /// <summary>
        /// 处理markdown
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private async Task MarkDown()
        {
            await InvokeAsync(StateHasChanged);
            await _JSRuntime.InvokeVoidAsync("Prism.highlightAll");
            await _JSRuntime.ScrollToBottomAsync("scrollDiv");
        }

        /// <summary>
        /// 上传文件事件
        /// </summary>
        /// <param name="fileInfo"></param>
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
        /// <summary>
        /// 移除文件事件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task<bool> HandleFileRemove(UploadFileItem file)
        {
            fileList.RemoveAll(x => x.FileName == file.FileName);
            await Task.Yield();
            return true;
        }
    }
}

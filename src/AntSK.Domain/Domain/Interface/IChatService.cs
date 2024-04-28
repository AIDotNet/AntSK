using AntSK.Domain.Domain.Model;
using AntSK.Domain.Domain.Model.Dto;
using AntSK.Domain.Repositories;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Interface
{
    public interface IChatService
    {
        IAsyncEnumerable<string> SendChatByAppAsync(Apps app, ChatHistory history);

        IAsyncEnumerable<StreamingKernelContent> SendKmsByAppAsync(Apps app, string questions, ChatHistory history, string filePath, List<RelevantSource> relevantSources = null);
        Task<string> SendImgByAppAsync(Apps app, string questions);
        Task<ChatHistory> GetChatHistory(List<Chats> MessageList, ChatHistory history);
    }
}
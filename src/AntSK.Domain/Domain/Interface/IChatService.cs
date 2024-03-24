using AntSK.Domain.Domain.Model;
using AntSK.Domain.Domain.Model.Dto;
using AntSK.Domain.Repositories;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Interface
{
    public interface IChatService
    {
        IAsyncEnumerable<StreamingKernelContent> SendChatByAppAsync(Apps app, string questions, ChatHistory history);

        IAsyncEnumerable<StreamingKernelContent> SendKmsByAppAsync(Apps app, string questions, ChatHistory history, string filePath, List<RelevantSource> relevantSources = null);

        Task<ChatHistory> GetChatHistory(List<MessageInfo> MessageList);
    }
}
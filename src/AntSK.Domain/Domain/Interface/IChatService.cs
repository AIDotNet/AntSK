using AntSK.Domain.Domain.Model.Dto;
using AntSK.Domain.Repositories;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Interface
{
    public interface IChatService
    {
        IAsyncEnumerable<StreamingKernelContent> SendChatByAppAsync(Apps app, string questions, string history);

        IAsyncEnumerable<StreamingKernelContent> SendKmsByAppAsync(Apps app, string questions, string history, string filePath, List<RelevantSource> relevantSources = null);
    }
}
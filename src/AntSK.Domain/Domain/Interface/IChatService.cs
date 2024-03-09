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
        IAsyncEnumerable<StreamingKernelContent> ChatByAppAsync(Apps app, string questions, string history);
    }
}

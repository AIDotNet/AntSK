using AntSK.Domain.Repositories;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Interface
{
    public interface IKernelService
    {
        Kernel GetKernelByApp(Apps app);
        void ImportFunctionsByApp(Apps app, Kernel _kernel);
        Task<string> HistorySummarize(Kernel _kernel, string questions, string history);
    }
}

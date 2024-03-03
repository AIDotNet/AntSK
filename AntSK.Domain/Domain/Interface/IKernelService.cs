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
        Kernel GetKernel(string modelId = null, string apiKey = null);
        void ImportFunctions(Apps app, Kernel _kernel);
    }
}

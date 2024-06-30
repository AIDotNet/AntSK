using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AntSK.Domain.Domain.Service.OllamaService;

namespace AntSK.Domain.Domain.Interface
{
    public interface IOllamaService
    {
        public event LogMessageHandler LogMessageReceived;
        Task StartOllama(string modelName);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Interface
{
    public interface ILLamaFactoryService
    {
        Task<bool> StartProcess(string modelName, string templateName);

        void KillProcess();
    }
}

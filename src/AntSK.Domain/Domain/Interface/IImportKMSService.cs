using AntSK.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Interface
{
    public interface IImportKMSService
    {
        void ImportKMSTask(ImportKMSTaskReq req);
    }
}

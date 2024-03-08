using AntSK.Domain.Domain.Dto;
using Microsoft.KernelMemory.Configuration;
using Microsoft.KernelMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Interface
{
    public interface IKMService
    {
        MemoryServerless GetMemory();
        MemoryServerless GetMemoryByKMS(string kmsID, SearchClientConfig searchClientConfig = null);
        Task<List<KMFile>> GetDocumentByFileID(string kmsid, string fileid);
    }
}

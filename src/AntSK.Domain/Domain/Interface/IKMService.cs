using AntSK.Domain.Domain.Model.Dto;
using Microsoft.KernelMemory;

namespace AntSK.Domain.Domain.Interface
{
    public interface IKMService
    {
        MemoryServerless GetMemoryByKMS(string kmsID, SearchClientConfig searchClientConfig = null);
        Task<List<KMFile>> GetDocumentByFileID(string kmsId, string fileId);

        Task<List<RelevantSource>> GetRelevantSourceList(string kmsIdListStr, string msg);
    }
}

using AntSK.Domain.Domain.Model.Dto;
using Microsoft.KernelMemory;

namespace AntSK.Domain.Domain.Interface
{
    public interface IKMService
    {
        MemoryServerless GetMemoryByKMS(string kmsID, SearchClientConfig searchClientConfig = null);
        Task<List<KMFile>> GetDocumentByFileID(string kmsid, string fileid);
    }
}

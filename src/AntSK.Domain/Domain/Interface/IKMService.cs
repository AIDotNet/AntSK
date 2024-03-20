using AntDesign;
using AntSK.Domain.Domain.Model.Dto;
using AntSK.Domain.Repositories;
using Microsoft.KernelMemory;

namespace AntSK.Domain.Domain.Interface
{
    public interface IKMService
    {
        MemoryServerless GetMemory(Apps app);

        MemoryServerless GetMemoryByKMS(string kmsID, SearchClientConfig searchClientConfig = null);

        Task<List<KMFile>> GetDocumentByFileID(string kmsId, string fileId);

        Task<List<RelevantSource>> GetRelevantSourceList(string kmsIdListStr, string msg);

        List<UploadFileItem> FileList { get; }

        bool BeforeUpload(UploadFileItem file);

        void OnSingleCompleted(UploadInfo fileinfo);
    }
}
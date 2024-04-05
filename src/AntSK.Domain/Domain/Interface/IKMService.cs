using AntDesign;
using AntSK.Domain.Domain.Model.Dto;
using AntSK.Domain.Repositories;
using Microsoft.KernelMemory;

namespace AntSK.Domain.Domain.Interface
{
    public interface IKMService
    {
        MemoryServerless GetMemoryByApp(Apps app);

        MemoryServerless GetMemoryByKMS(string kmsID);

        Task<List<KMFile>> GetDocumentByFileID(string kmsId, string fileId);

        Task<List<RelevantSource>> GetRelevantSourceList(Apps app, string msg);

        List<UploadFileItem> FileList { get; }

        bool BeforeUpload(UploadFileItem file);

        void OnSingleCompleted(UploadInfo fileinfo);
    }
}
using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Domain.Interface;
using Microsoft.KernelMemory;
using AntSK.Domain.Utils;
using AntSK.Domain.Domain.Dto;

namespace AntSK.Domain.Domain.Service
{
    [ServiceDescription(typeof(IKMService), ServiceLifetime.Scoped)]
    public class KMService(MemoryServerless _memory) : IKMService
    {
        public async Task<List<KMFile>> GetDocumentByFileID(string fileid)
        {
            var memories = await _memory.ListIndexesAsync();
            var memoryDbs = _memory.Orchestrator.GetMemoryDbs();
            List<KMFile> docTextList = new List<KMFile>();

            foreach (var memoryIndex in memories)
            {
                foreach (var memoryDb in memoryDbs)
                {
                    var list = memoryDb.GetListAsync(memoryIndex.Name, null, 100, true);

                    await foreach (var item in list)
                    {
                        if (item.Id.Contains(fileid))
                        {
      
                            KMFile file = new KMFile()
                            {
                                Text = item.Payload.FirstOrDefault(p => p.Key == "text").Value.ConvertToString(),
                                Url= item.Payload.FirstOrDefault(p => p.Key == "url").Value.ConvertToString(),
                                LastUpdate= item.Payload.FirstOrDefault(p => p.Key == "last_update").Value.ConvertToString(),
                                Schema = item.Payload.FirstOrDefault(p => p.Key == "schema").Value.ConvertToString(),
                                File = item.Payload.FirstOrDefault(p => p.Key == "file").Value.ConvertToString(),
                            };
                            docTextList.Add(file);
                        }
                    }
                }
            }
            return docTextList;
        }
    }
}

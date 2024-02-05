using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Domain.Interface;
using Microsoft.KernelMemory;
using AntSK.Domain.Utils;

namespace AntSK.Domain.Domain.Service
{
    [ServiceDescription(typeof(IKMService), ServiceLifetime.Scoped)]
    public class KMService(MemoryServerless _memory) : IKMService
    {
        public async Task<List<string>> GetDocumentByFileID(string fileid)
        {
            var memories = await _memory.ListIndexesAsync();
            var memoryDbs = _memory.Orchestrator.GetMemoryDbs();
            List<string> docTextList = new List<string>();

            foreach (var memoryIndex in memories)
            {
                foreach (var memoryDb in memoryDbs)
                {
                    var list = memoryDb.GetListAsync(memoryIndex.Name, null, 100, true);

                    await foreach (var item in list)
                    {
                        if (item.Id.Contains(fileid))
                        {
                            var test = item.Payload.FirstOrDefault(p => p.Key == "text");
                            docTextList.Add(test.Value.ConvertToString());
                        }
                    }
                }
            }
            return docTextList;
        }
    }
}

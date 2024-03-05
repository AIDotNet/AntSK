using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Domain.Interface;
using Microsoft.KernelMemory;
using AntSK.Domain.Utils;
using AntSK.Domain.Domain.Dto;
using AntSK.Domain.Options;
using Microsoft.KernelMemory.ContentStorage.DevTools;
using Microsoft.KernelMemory.FileSystem.DevTools;
using Microsoft.KernelMemory.Postgres;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Microsoft.KernelMemory.Configuration;

namespace AntSK.Domain.Domain.Service
{
    [ServiceDescription(typeof(IKMService), ServiceLifetime.Scoped)]
    public class KMService(
        IOptions<PostgresConfig> postgresOptions
        ) : IKMService
    {
        public MemoryServerless GetMemory(SearchClientConfig searchClientConfig=null, TextPartitioningOptions textPartitioningOptions=null) 
        {
            var handler = new OpenAIHttpClientHandler();
            var httpClient = new HttpClient(handler);
            if (searchClientConfig.IsNull())
            {
                 searchClientConfig = new SearchClientConfig
                {
                    MaxAskPromptSize = 2048,
                    MaxMatchesCount = 3,
                    AnswerTokens = 1000,
                    EmptyAnswer = "知识库未搜索到相关内容"
                };
            }

            if (textPartitioningOptions.IsNull())
            {
                textPartitioningOptions = new TextPartitioningOptions
                {
                    MaxTokensPerLine = 99,
                    MaxTokensPerParagraph = 299,
                    OverlappingTokens = 47
                };
            }

           var memory = new KernelMemoryBuilder()
          .WithPostgresMemoryDb(postgresOptions.Value)
          .WithSimpleFileStorage(new SimpleFileStorageConfig { StorageType = FileSystemTypes.Volatile, Directory = "_files" })
          .WithSearchClientConfig(searchClientConfig)
          //如果用本地模型需要设置token小一点。
          .WithCustomTextPartitioningOptions(textPartitioningOptions)
          .WithOpenAITextGeneration(new OpenAIConfig()
          {
              APIKey = OpenAIOption.Key,
              TextModel = OpenAIOption.Model

          }, null, httpClient)
          .WithOpenAITextEmbeddingGeneration(new OpenAIConfig()
          {
              APIKey = OpenAIOption.Key,
              EmbeddingModel = OpenAIOption.EmbeddingModel

          }, null, false, httpClient)
          .Build<MemoryServerless>();
            return memory;
        }

        public async Task<List<KMFile>> GetDocumentByFileID(string fileid)
        {
            var _memory = GetMemory();
            var memories = await _memory.ListIndexesAsync();
            var memoryDbs = _memory.Orchestrator.GetMemoryDbs();
            List<KMFile> docTextList = new List<KMFile>();

            foreach (var memoryIndex in memories)
            {
                foreach (var memoryDb in memoryDbs)
                {

                    var items = await memoryDb.GetListAsync(memoryIndex.Name, new List<MemoryFilter>() { new MemoryFilter().ByDocument(fileid) }, 100, true).ToListAsync();
                    foreach (var item in items)
                    {
                        KMFile file = new KMFile()
                        {
                            Text = item.Payload.FirstOrDefault(p => p.Key == "text").Value.ConvertToString(),
                            Url = item.Payload.FirstOrDefault(p => p.Key == "url").Value.ConvertToString(),
                            LastUpdate = item.Payload.FirstOrDefault(p => p.Key == "last_update").Value.ConvertToString(),
                            Schema = item.Payload.FirstOrDefault(p => p.Key == "schema").Value.ConvertToString(),
                            File = item.Payload.FirstOrDefault(p => p.Key == "file").Value.ConvertToString(),
                        };
                        docTextList.Add(file);
                    }
                }
            }
            return docTextList;
        }
    }
}

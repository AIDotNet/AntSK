using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Domain.Model.Constant;
using AntSK.Domain.Domain.Model.Dto;
using AntSK.Domain.Domain.Other;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using LLama;
using LLamaSharp.KernelMemory;
using Markdig;
using Microsoft.Extensions.Configuration;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Configuration;
using Microsoft.KernelMemory.FileSystem.DevTools;
using Microsoft.KernelMemory.MemoryStorage;
using Microsoft.KernelMemory.MemoryStorage.DevTools;
using Microsoft.KernelMemory.Postgres;

namespace AntSK.Domain.Domain.Service
{
    [ServiceDescription(typeof(IKMService), ServiceLifetime.Scoped)]
    public class KMService(
        IConfiguration _config,
        IKmss_Repositories _kmss_Repositories,
        IAIModels_Repositories _aIModels_Repositories
    ) : IKMService
    {
        private MemoryServerless _memory;

        public MemoryServerless GetMemoryByKMS(string kmsID, SearchClientConfig searchClientConfig = null)
        {
            //if (_memory.IsNull())
            {
                //获取KMS配置
                var kms = _kmss_Repositories.GetFirst(p => p.Id == kmsID);
                var chatModel = _aIModels_Repositories.GetFirst(p => p.Id == kms.ChatModelID);
                var embedModel = _aIModels_Repositories.GetFirst(p => p.Id == kms.EmbeddingModelID);

                //http代理
                var chatHttpClient = OpenAIHttpClientHandlerUtil.GetHttpClient(chatModel.EndPoint);
                var embeddingHttpClient = OpenAIHttpClientHandlerUtil.GetHttpClient(embedModel.EndPoint);

                //搜索配置
                if (searchClientConfig.IsNull())
                {
                    searchClientConfig = new SearchClientConfig
                    {
                        MaxAskPromptSize = 2048,
                        MaxMatchesCount = 3,
                        AnswerTokens = 1000,
                        EmptyAnswer = KmsConstantcs.KmsSearchNull
                    };
                }

                var memoryBuild = new KernelMemoryBuilder()
                    .WithSearchClientConfig(searchClientConfig)
                    .WithCustomTextPartitioningOptions(new TextPartitioningOptions
                    {
                        MaxTokensPerLine = kms.MaxTokensPerLine,
                        MaxTokensPerParagraph = kms.MaxTokensPerParagraph,
                        OverlappingTokens = kms.OverlappingTokens
                    });
                //加载会话模型
                WithTextGenerationByAIType(memoryBuild, chatModel, chatHttpClient);
                //加载向量模型
                WithTextEmbeddingGenerationByAIType(memoryBuild, embedModel, embeddingHttpClient);
                //加载向量库
                WithMemoryDbByVectorDB(memoryBuild, _config);

                _memory = memoryBuild.Build<MemoryServerless>();
                return _memory;
            }
            //else {
            //    return _memory;
            //}
        }

        private void WithTextEmbeddingGenerationByAIType(IKernelMemoryBuilder memory, AIModels embedModel,
            HttpClient embeddingHttpClient)
        {
            switch (embedModel.AIType)
            {
                case Model.Enum.AIType.OpenAI:
                    memory.WithOpenAITextEmbeddingGeneration(new OpenAIConfig()
                    {
                        APIKey = embedModel.ModelKey,
                        EmbeddingModel = embedModel.ModelName
                    }, null, false, embeddingHttpClient);
                    break;
                case Model.Enum.AIType.AzureOpenAI:
                    memory.WithAzureOpenAITextEmbeddingGeneration(new AzureOpenAIConfig()
                    {
                        APIKey = embedModel.ModelKey,
                        Deployment = embedModel.ModelName.ConvertToString(),
                        Endpoint = embedModel.EndPoint.ConvertToString(),
                        Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                        APIType = AzureOpenAIConfig.APITypes.EmbeddingGeneration,
                    });
                    break;
                case Model.Enum.AIType.LLamaSharp:
                    var (weights, parameters) = LLamaConfig.GetLLamaConfig(embedModel.ModelName);
                    var embedder = new LLamaEmbedder(weights, parameters);
                    memory.WithLLamaSharpTextEmbeddingGeneration(new LLamaSharpTextEmbeddingGenerator(embedder));
                    break;
                case Model.Enum.AIType.DashScope:
                    memory.WithDashScopeDefaults(embedModel.ModelKey);
                    break;
            }
        }

        private void WithTextGenerationByAIType(IKernelMemoryBuilder memory, AIModels chatModel,
            HttpClient chatHttpClient)
        {
            switch (chatModel.AIType)
            {
                case Model.Enum.AIType.OpenAI:
                    memory.WithOpenAITextGeneration(new OpenAIConfig()
                    {
                        APIKey = chatModel.ModelKey,
                        TextModel = chatModel.ModelName
                    }, null, chatHttpClient);
                    break;
                case Model.Enum.AIType.AzureOpenAI:
                    memory.WithAzureOpenAITextGeneration(new AzureOpenAIConfig()
                    {
                        APIKey = chatModel.ModelKey,
                        Deployment = chatModel.ModelName.ConvertToString(),
                        Endpoint = chatModel.EndPoint.ConvertToString(),
                        Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                        APIType = AzureOpenAIConfig.APITypes.TextCompletion,
                    });
                    break;
                case Model.Enum.AIType.LLamaSharp:
                    var (weights, parameters) = LLamaConfig.GetLLamaConfig(chatModel.ModelName);
                    var context = weights.CreateContext(parameters);
                    var executor = new StatelessExecutor(weights, parameters);
                    memory.WithLLamaSharpTextGeneration(new LlamaSharpTextGenerator(weights, context, executor));
                    break;
                case Model.Enum.AIType.DashScope:
                    memory.WithDashScopeTextGeneration(new Cnblogs.KernelMemory.AI.DashScope.DashScopeConfig
                    {
                        ApiKey = chatModel.ModelKey,
                    });
                    break;
            }
        }

        private void WithMemoryDbByVectorDB(IKernelMemoryBuilder memory, IConfiguration _config)
        {
            string VectorDb = _config["KernelMemory:VectorDb"].ConvertToString();
            string ConnectionString = _config["KernelMemory:ConnectionString"].ConvertToString();
            string TableNamePrefix = _config["KernelMemory:TableNamePrefix"].ConvertToString();
            switch (VectorDb)
            {
                case "Postgres":
                    memory.WithPostgresMemoryDb(new PostgresConfig()
                    {
                        ConnectionString = ConnectionString,
                        TableNamePrefix = TableNamePrefix
                    });
                    break;
                case "Disk":
                    memory.WithSimpleVectorDb(new SimpleVectorDbConfig()
                    {
                        StorageType = FileSystemTypes.Disk,
                    });
                    break;
                case "Memory":
                    memory.WithSimpleVectorDb(new SimpleVectorDbConfig()
                    {
                        StorageType = FileSystemTypes.Volatile
                    });
                    break;
            }
        }

        public async Task<List<KMFile>> GetDocumentByFileID(string kmsId, string fileId)
        {
            var memory = GetMemoryByKMS(kmsId);
            var memories = await memory.ListIndexesAsync();
            var memoryDbs = memory.Orchestrator.GetMemoryDbs();
            var docTextList = new List<KMFile>();

            foreach (var memoryIndex in memories)
            {
                foreach (var memoryDb in memoryDbs)
                {
                    var items = await memoryDb.GetListAsync(memoryIndex.Name, new List<MemoryFilter>() { new MemoryFilter().ByDocument(fileId) }, 100, true).ToListAsync();
                    docTextList.AddRange(items.Select(item => new KMFile()
                    {
                        DocumentId = item.GetDocumentId(),
                        Text = item.GetPartitionText(),
                        Url = item.GetWebPageUrl(),
                        LastUpdate = item.GetLastUpdate().LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        File = item.GetFileName()
                    }));
                }
            }

            return docTextList;
        }

        public async Task<List<RelevantSource>> GetRelevantSourceList(string kmsIdListStr, string msg)
        {
            var result = new List<RelevantSource>();
            var kmsIdList = kmsIdListStr.Split(",");
            if (!kmsIdList.Any()) return result;

            var memory = GetMemoryByKMS(kmsIdList.FirstOrDefault()!);

            var filters = kmsIdList.Select(kmsId => new MemoryFilter().ByTag(KmsConstantcs.KmsIdTag, kmsId)).ToList();

            var searchResult = await memory.SearchAsync(msg, index: KmsConstantcs.KmsIndex, filters: filters);
            if (!searchResult.NoResult) 
            {
                foreach (var item in searchResult.Results)
                {
                    result.AddRange(item.Partitions.Select(part => new RelevantSource()
                    {
                        SourceName = item.SourceName,
                        Text = Markdown.ToHtml(part.Text),
                        Relevance = part.Relevance
                    }));
                }
            }

            return result;
        }
    }
}
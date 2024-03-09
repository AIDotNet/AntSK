using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Options;
using LLama;
using LLama.Common;

namespace AntSK.Services.LLamaSharp
{
    public interface ILLamaEmbeddingService
    {
        Task<List<float>> Embedding(string text);
    }

    /// <summary>
    /// 本地Embedding
    /// </summary>
    [ServiceDescription(typeof(ILLamaEmbeddingService), Domain.Common.DependencyInjection.ServiceLifetime.Singleton)]
    public class LLamaEmbeddingService : IDisposable, ILLamaEmbeddingService
    {
        private LLamaEmbedder _embedder;

        public LLamaEmbeddingService()
        {

            var @params = new ModelParams(LLamaSharpOption.Embedding) { EmbeddingMode = true };
            using var weights = LLamaWeights.LoadFromFile(@params);
            _embedder = new LLamaEmbedder(weights, @params);
        }
        public void Dispose()
        {
            _embedder?.Dispose();
        }

        public async Task<List<float>> Embedding(string text)
        {
            float[] embeddings = await _embedder.GetEmbeddings(text);
            //PG只有1536维
            return embeddings.ToList();
        }
    }
}

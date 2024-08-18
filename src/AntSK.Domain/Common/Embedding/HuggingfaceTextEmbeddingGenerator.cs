using Microsoft.KernelMemory.AI;
using AntSK.Domain.Domain.Other.Bge;

namespace AntSK.Domain.Common.Embedding
{
    public class HuggingfaceTextEmbeddingGenerator : ITextEmbeddingGenerator, ITextTokenizer, IDisposable
    {
        public int MaxTokens => 1024;

        public int MaxTokenTotal => 1024;

  
        private readonly dynamic _embedder;

        public HuggingfaceTextEmbeddingGenerator(string pyDllPath,string modelName)
        {
            _embedder = BgeEmbeddingConfig.LoadModel(pyDllPath, modelName);
        }

        public void Dispose()
        {
            BgeEmbeddingConfig.Dispose();
        }

        //public async Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingAsync(IList<string> data, CancellationToken cancellationToken = default)
        //{
        //    IList<ReadOnlyMemory<float>> results = new List<ReadOnlyMemory<float>>();

        //    foreach (var d in data)
        //    {
        //        var embeddings = await EmbeddingConfig.GetEmbedding(d);
        //        results.Add(new ReadOnlyMemory<float>(embeddings));
        //    }
        //    return results;
        //}

        public async Task<Microsoft.KernelMemory.Embedding> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
        {
            var embeddings = await BgeEmbeddingConfig.GetEmbedding(text);
            return new Microsoft.KernelMemory.Embedding(embeddings);
        }

        public int CountTokens(string text)
        {
            return BgeEmbeddingConfig.TokenCount(text);
        }

        public IReadOnlyList<string> GetTokens(string text)
        {
            return new List<string>();
        }
    }
}

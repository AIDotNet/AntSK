using Microsoft.KernelMemory.AI;
using Microsoft.KernelMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Common.Embedding
{
    public static class BuilderBgeExtensions
    {
        public static IKernelMemoryBuilder WithBgeTextEmbeddingGeneration(this IKernelMemoryBuilder builder, HuggingfaceTextEmbeddingGenerator textEmbeddingGenerator)
        {
            builder.AddSingleton((ITextEmbeddingGenerator)textEmbeddingGenerator);
            builder.AddIngestionEmbeddingGenerator(textEmbeddingGenerator);
            return builder;
        }
    }
}

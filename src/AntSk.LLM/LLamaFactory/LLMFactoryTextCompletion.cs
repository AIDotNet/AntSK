using AntSK.LLM.SparkDesk;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Services;
using Microsoft.SemanticKernel.TextGeneration;
using Sdcb.SparkDesk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Runtime.Intrinsics.Arm;
using RestSharp;
using System.Text.Json.Nodes;
using Newtonsoft.Json;

namespace AntSK.LLM.LLamaFactory
{
    public class LLMFactoryTextCompletion : ITextGenerationService, IAIService
    {
        private readonly Dictionary<string, object?> _attributes = new();
        private string _chatId;
        private readonly LLMFactoryOptions _options;

        public IReadOnlyDictionary<string, object?> Attributes => _attributes;

        public LLMFactoryTextCompletion(LLMFactoryOptions options, string chatId)
        {
            _options = options;
            _chatId = chatId;
        }

        public Task<IReadOnlyList<TextContent>> GetTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();

        }

        public IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}

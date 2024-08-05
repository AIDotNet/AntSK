using AntSK.LLM.SparkDesk;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Services;
using Microsoft.SemanticKernel.TextGeneration;
using Sdcb.SparkDesk;
using System;
using System.ComponentModel;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace AntSK.LLM.Mock
{
    public class MockTextCompletion : ITextGenerationService, IAIService
    {
        private readonly Dictionary<string, object?> _attributes = new();
        private readonly SparkDeskClient _client;
        private string _chatId;
        private readonly SparkDeskOptions _options;

        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public IReadOnlyDictionary<string, object?> Attributes => _attributes;

        public MockTextCompletion()
        {
         
        }

        public async Task<IReadOnlyList<TextContent>> GetTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            StringBuilder sb = new();
            string result = $"这是一条Mock数据，便于聊天测试，你的消息是：{prompt}";
            return [new(result.ToString())];
        }

        public async IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            StringBuilder sb = new();
            string result = $"这是一条Mock数据，便于聊天测试，你的消息是：{prompt}";
            foreach (var c in result)
            {
                var streamingTextContent = new StreamingTextContent(c.ToString(), modelId: "mock");

                yield return streamingTextContent;
            }
        }
    }
}
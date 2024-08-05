using AntSK.LLM.SparkDesk;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Sdcb.SparkDesk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace AntSK.LLM.Mock
{
    public class MockChatCompletion : IChatCompletionService
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

        public MockChatCompletion()
        {

        }

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            StringBuilder sb = new();
            string result = $"这是一条Mock数据，便于聊天测试，你的消息是：{chatHistory.LastOrDefault().ToString()}";
            return [new(AuthorRole.Assistant, result.ToString())];
        }

        public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            StringBuilder sb = new();
            string result = $"这是一条Mock数据，便于聊天测试，你的消息是：{chatHistory.LastOrDefault().ToString()}";
            foreach (var c in result)
            {
                yield return new StreamingChatMessageContent(AuthorRole.Assistant, c.ToString());
            }
        }
    }
}

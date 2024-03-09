using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Services;
using Microsoft.SemanticKernel.TextGeneration;
using Sdcb.SparkDesk;
using System.Text;

namespace AntSk.LLM.SparkDesk
{
    public class SparkDeskTextCompletion : ITextGenerationService, IAIService
    {
        private readonly Dictionary<string, object?> _attributes = new();

        public IReadOnlyDictionary<string, object?> Attributes => _attributes;

        private SparkDeskClient _client;

        private string _chatId;

        SparkDeskOptions _options;

        public SparkDeskTextCompletion(SparkDeskOptions options, string chatId)
        {
            _options = options;
            _chatId = chatId;
            _client = new(options.AppId, options.ApiKey, options.ApiSecret);
        }

        public async Task<IReadOnlyList<TextContent>> GetTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            StringBuilder sb = new();
            var parameters = new ChatRequestParameters
            {
                ChatId = _chatId,
            };

            if (executionSettings is OpenAIPromptExecutionSettings openAISettings)
            {
                parameters.Temperature = (float)openAISettings.Temperature;
                parameters.MaxTokens = openAISettings.MaxTokens ?? parameters.MaxTokens;
            }

            await foreach (StreamedChatResponse msg in _client.ChatAsStreamAsync(_options.ModelVersion, GetHistories(prompt), parameters))
            {
                sb.Append(msg);
            };

            return [new(sb.ToString())];
        }

        public async IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            var parameters = new ChatRequestParameters
            {
                ChatId = _chatId,
            };

            if (executionSettings is OpenAIPromptExecutionSettings openAISettings)
            {
                parameters.Temperature = (float)openAISettings.Temperature;
                parameters.MaxTokens = openAISettings.MaxTokens ?? parameters.MaxTokens;
            }

            await foreach (StreamedChatResponse msg in _client.ChatAsStreamAsync(_options.ModelVersion, GetHistories(prompt), parameters, cancellationToken: cancellationToken))
            {
                yield return new(msg);
            };
        }

        private ChatMessage[] GetHistories(string prompt)
        {
            var histories = prompt.Replace("history：", "")
                .Split("\r\n")
                .Select(m => m.Split(":", 2))
                .Where(m => m.Length == 2)
                .Select(pair => new ChatMessage(pair[0].Trim() == "user" ? "user" : "assistant", pair[1])).ToArray();

            return histories;
        }
    }
}

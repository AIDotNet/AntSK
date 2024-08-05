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

namespace AntSK.LLM.SparkDesk
{
    public class SparkDeskTextCompletion : ITextGenerationService, IAIService
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

            OpenAIPromptExecutionSettings chatExecutionSettings = OpenAIPromptExecutionSettings.FromExecutionSettings(executionSettings);

            parameters.Temperature = (float)chatExecutionSettings.Temperature;
            parameters.MaxTokens = chatExecutionSettings.MaxTokens ?? parameters.MaxTokens;

            await foreach (StreamedChatResponse msg in _client.ChatAsStreamAsync(_options.ModelVersion, GetHistories(prompt), parameters))
            {
                sb.Append(msg);
            };

            return [new(sb.ToString())];
        }

        public IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            var parameters = new ChatRequestParameters
            {
                ChatId = _chatId,
            };
            OpenAIPromptExecutionSettings chatExecutionSettings = OpenAIPromptExecutionSettings.FromExecutionSettings(executionSettings);

            parameters.Temperature = (float)chatExecutionSettings.Temperature;
            parameters.MaxTokens = chatExecutionSettings.MaxTokens ?? parameters.MaxTokens;

            IList<KernelFunctionMetadata> functions = kernel?.Plugins.GetFunctionsMetadata().Where(x => x.PluginName == "AntSKFunctions").ToList() ?? [];
            var functionDefs = functions.Select(func => new FunctionDef(func.Name, func.Description, func.Parameters.Select(p => new FunctionParametersDef(p.Name, p.ParameterType?.IsClass == true ? "object" : "string", p.Description, p.IsRequired)).ToList())).ToList();

            //var messages = GetHistories(prompt);
            var messages = new ChatMessage[] { new ChatMessage("user", prompt) };

            return GetStreamingMessageAsync(messages, parameters, functionDefs, cancellationToken);

            async IAsyncEnumerable<StreamingTextContent> GetStreamingMessageAsync(ChatMessage[] messages, ChatRequestParameters parameters, List<FunctionDef> functionDefs, CancellationToken cancellationToken)
            {
                await foreach (StreamedChatResponse msg in _client.ChatAsStreamAsync(_options.ModelVersion, messages, parameters, functionDefs.Count > 0 ? [.. functionDefs] : null, cancellationToken: cancellationToken))
                {
                    if (msg.FunctionCall != null)
                    {
                        var func = functions.Where(x => x.Name == msg.FunctionCall.Name).FirstOrDefault();

                        if (func == null)
                        {
                            yield return new($"插件{msg.FunctionCall.Name}未注册");
                            yield break;
                        }

                        if (kernel.Plugins.TryGetFunction(func.PluginName, func.Name, out var function))
                        {
                            var arguments = new KernelArguments();

                            var JsonElement = JsonDocument.Parse(msg.FunctionCall.Arguments).RootElement;
                            foreach (var parameter in func.Parameters)
                            {
                                var error = "";
                                try
                                {
                                    if (JsonElement.TryGetProperty(parameter.Name, out var property))
                                    {
                                        arguments.Add(parameter.Name, property.Deserialize(parameter.ParameterType!, _jsonSerializerOptions));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    error = $"参数{parameter.Name}解析错误:{ex.Message}";
                                }

                                if (!string.IsNullOrEmpty(error))
                                {
                                    yield return new(error);
                                    yield break;
                                }
                            }

                            var result = (await function.InvokeAsync(kernel, arguments, cancellationToken)).GetValue<object>() ?? string.Empty;
                            var stringResult = ProcessFunctionResult(result, chatExecutionSettings.ToolCallBehavior);
                            messages = [ChatMessage.FromSystem($"""
                                用户意图{func.Description}结果是：{stringResult}

                                请结合用户的提问回复：
                                """), ChatMessage.FromUser(prompt)];

                            functionDefs.Clear();

                            await foreach (var content in GetStreamingMessageAsync(messages, parameters, functionDefs, cancellationToken))
                            {
                                yield return content;
                            }
                        }
                    }
                    else
                    {
                        yield return new(msg);
                    }
                };
            }
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

        private static string? ProcessFunctionResult(object functionResult, ToolCallBehavior? toolCallBehavior)
        {
            if (functionResult is string stringResult)
            {
                return stringResult;
            }

            if (functionResult is ChatMessageContent chatMessageContent)
            {
                return chatMessageContent.ToString();
            }

            return JsonSerializer.Serialize(functionResult, _jsonSerializerOptions);
        }

        public static Dictionary<string, object> ParseJsonElement(JsonElement element, string propertyName)
        {
            Dictionary<string, object> dict = new();

            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (JsonProperty property in element.EnumerateObject())
                    {
                        dict.Add(property.Name, ParseJsonElement(property.Value, property.Name));
                    }
                    break;

                case JsonValueKind.Array:
                    List<object> list = new List<object>();
                    foreach (JsonElement arrayElement in element.EnumerateArray())
                    {
                        list.Add(ParseJsonElement(arrayElement, ""));
                    }
                    dict.Add(propertyName, list);
                    break;

                case JsonValueKind.String:
                    dict.Add(propertyName, element.GetString());
                    break;

                case JsonValueKind.Number:
                    dict.Add(propertyName, element.GetInt32());
                    break;

                case JsonValueKind.True:
                case JsonValueKind.False:
                    dict.Add(propertyName, element.GetBoolean());
                    break;

                default:
                    dict.Add(propertyName, "Unsupported value type");
                    break;
            }

            return dict;
        }
    }
}
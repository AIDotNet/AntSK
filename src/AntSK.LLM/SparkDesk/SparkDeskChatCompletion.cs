using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
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

namespace AntSK.LLM.SparkDesk
{
    public class SparkDeskChatCompletion : IChatCompletionService
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

        public SparkDeskChatCompletion(SparkDeskOptions options, string chatId)
        {
            _options = options;
            _chatId = chatId;
            _client = new(options.AppId, options.ApiKey, options.ApiSecret);
        }

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            StringBuilder sb = new();
            var parameters = new ChatRequestParameters
            {
                ChatId = _chatId,
            };

            OpenAIPromptExecutionSettings chatExecutionSettings = OpenAIPromptExecutionSettings.FromExecutionSettings(executionSettings);

            parameters.Temperature = (float)chatExecutionSettings.Temperature;
            parameters.MaxTokens = chatExecutionSettings.MaxTokens ?? parameters.MaxTokens;

            IList<KernelFunctionMetadata> functions = kernel?.Plugins.GetFunctionsMetadata().Where(x => x.PluginName == "AntSKFunctions").ToList() ?? [];
            var functionDefs = functions.Select(func => new FunctionDef(func.Name, func.Description, func.Parameters.Select(p => new FunctionParametersDef(p.Name, p.ParameterType?.IsClass == true ? "object" : "string", p.Description, p.IsRequired)).ToList())).ToList();

            List<ChatMessage> messages = GetSparkMessage(chatHistory);

            var result = await _client.ChatAsync(_options.ModelVersion, messages.ToArray(), parameters, functionDefs.Count > 0 ? [.. functionDefs] : null, cancellationToken: cancellationToken);

            if (result.FunctionCall != null)
            {
                var func = functions.Where(x => x.Name == result.FunctionCall.Name).FirstOrDefault();

                if (func == null)
                {
                    return new List<ChatMessageContent> { new(AuthorRole.Assistant, $"插件{result.FunctionCall.Name}未注册") }.AsReadOnly();
                }

                if (kernel.Plugins.TryGetFunction(func.PluginName, func.Name, out var function))
                {
                    var arguments = new KernelArguments();

                    var JsonElement = JsonDocument.Parse(result.FunctionCall.Arguments).RootElement;
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
                            return new List<ChatMessageContent> { new(AuthorRole.Assistant, error) }.AsReadOnly();

                        }
                    }
                    var functionResult = await function.InvokeAsync(kernel, arguments, cancellationToken);
                    messages = [ ChatMessage.FromUser(messages.LastOrDefault().Content),
                    ChatMessage.FromSystem($@"
                                执行函数调用成功
                                函数描述：{func.Description}
                                函数执行结果：{functionResult}
                                "),
                    ChatMessage.FromUser("请根据函数调用结果回答我的问题，不要超出函数调用结果的返回，以及不要有多余描述：")];


                    var callResult = await _client.ChatAsync(_options.ModelVersion, messages.ToArray(), parameters, null);
                    ChatMessageContent chatMessageContent = new(AuthorRole.Assistant, callResult.Text.ToString(), modelId: "SparkDesk");

                    return new List<ChatMessageContent> { chatMessageContent }.AsReadOnly();

                }
                return new List<ChatMessageContent> { new(AuthorRole.Assistant, "未找到插件") }.AsReadOnly();

            }
            else
            {

                ChatMessageContent chatMessageContent = new(AuthorRole.Assistant, result.Text.ToString(), modelId: "SparkDesk");

                return new List<ChatMessageContent> { chatMessageContent }.AsReadOnly();
            }

        }


        public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
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
            List<ChatMessage> messages = GetSparkMessage(chatHistory);
            await foreach (StreamedChatResponse msg in _client.ChatAsStreamAsync(_options.ModelVersion, messages.ToArray(), parameters, functionDefs.Count > 0 ? [.. functionDefs] : null, cancellationToken: cancellationToken))
            {

                yield return new StreamingChatMessageContent(AuthorRole.Assistant, msg);

            };

        }

        private static List<ChatMessage> GetSparkMessage(ChatHistory chatHistory)
        {
            List<ChatMessage> messages = new List<ChatMessage>();
            foreach (var msg in chatHistory.ToList())
            {
                string role = "";
                if (msg.Role == AuthorRole.User)
                {
                    role = "user";
                }
                else if (msg.Role == AuthorRole.System)
                {
                    role = "system";
                }
                else
                {
                    role = "assistant";
                }
                messages.Add(new ChatMessage(role, msg.ToString()));
            }

            return messages;
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

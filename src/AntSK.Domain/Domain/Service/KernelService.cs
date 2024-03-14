using AntSK.LLM.SparkDesk;
using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Domain.Other;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using LLama;
using LLamaSharp.SemanticKernel.TextCompletion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.TextGeneration;
using RestSharp;
using System;
using ServiceLifetime = AntSK.Domain.Common.DependencyInjection.ServiceLifetime;
using AntSK.LLM.Mock;
using AntSK.Domain.Domain.Model.Enum;

namespace AntSK.Domain.Domain.Service
{
    [ServiceDescription(typeof(IKernelService), ServiceLifetime.Scoped)]
    public class KernelService : IKernelService
    {
        private readonly IApis_Repositories _apis_Repositories;
        private readonly IAIModels_Repositories _aIModels_Repositories;
        private readonly FunctionService _functionService;
        private readonly IServiceProvider _serviceProvider;
        private Kernel _kernel;

        public KernelService(
              IApis_Repositories apis_Repositories,
              IAIModels_Repositories aIModels_Repositories,
              FunctionService functionService,
              IServiceProvider serviceProvider)
        {
            _apis_Repositories = apis_Repositories;
            _aIModels_Repositories = aIModels_Repositories;
            _functionService = functionService;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 获取kernel实例，依赖注入不好按每个用户去Import不同的插件，所以每次new一个新的kernel
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public Kernel GetKernelByApp(Apps app)
        {
            //if (_kernel.IsNull())
            {
                var chatModel = _aIModels_Repositories.GetFirst(p => p.Id == app.ChatModelID);

                var chatHttpClient = OpenAIHttpClientHandlerUtil.GetHttpClient(chatModel.EndPoint);

                var builder = Kernel.CreateBuilder();
                WithTextGenerationByAIType(builder, app, chatModel, chatHttpClient);

                _kernel = builder.Build();
                RegisterPluginsWithKernel(_kernel);
                return _kernel;
            }
            //else 
            //{
            //    return _kernel;
            //}
        }

        private void WithTextGenerationByAIType(IKernelBuilder builder, Apps app, AIModels chatModel, HttpClient chatHttpClient)
        {
            switch (chatModel.AIType)
            {
                case Model.Enum.AIType.OpenAI:
                    builder.AddOpenAIChatCompletion(
                       modelId: chatModel.ModelName,
                       apiKey: chatModel.ModelKey,
                       httpClient: chatHttpClient);
                    break;

                case Model.Enum.AIType.AzureOpenAI:
                    builder.AddAzureOpenAIChatCompletion(
                        deploymentName: chatModel.ModelName,
                        apiKey: chatModel.ModelKey,
                        endpoint: chatModel.EndPoint
                        );
                    break;

                case Model.Enum.AIType.LLamaSharp:
                    var (weights, parameters) = LLamaConfig.GetLLamaConfig(chatModel.ModelName);
                    var ex = new StatelessExecutor(weights, parameters);
                    builder.Services.AddKeyedSingleton<ITextGenerationService>("local-llama", new LLamaSharpTextCompletion(ex));
                    break;

                case Model.Enum.AIType.SparkDesk:
                    var options = new SparkDeskOptions { AppId = chatModel.EndPoint, ApiSecret = chatModel.ModelKey, ApiKey = chatModel.ModelName, ModelVersion = Sdcb.SparkDesk.ModelVersion.V3_5 };
                    builder.Services.AddKeyedSingleton<ITextGenerationService>("spark-desk", new SparkDeskTextCompletion(options, app.Id));
                    break;
                case Model.Enum.AIType.Mock:
                    builder.Services.AddKeyedSingleton<ITextGenerationService>("mock", new MockTextCompletion());
                    break;
            }
        }

        /// <summary>
        /// 根据app配置的插件，导入插件
        /// </summary>
        /// <param name="app"></param>
        /// <param name="_kernel"></param>
        public void ImportFunctionsByApp(Apps app, Kernel _kernel)
        {
            //插件不能重复注册，否则会异常
            if (_kernel.Plugins.Any(p => p.Name == "AntSkFunctions"))
            {
                return;
            }
            List<KernelFunction> apiFunctions = new List<KernelFunction>();

            //API插件
            if (!string.IsNullOrWhiteSpace(app.ApiFunctionList))
            {
                //开启自动插件调用
                var apiIdList = app.ApiFunctionList.Split(",");
                var apiList = _apis_Repositories.GetList(p => apiIdList.Contains(p.Id));

                foreach (var api in apiList)
                {
                    switch (api.Method)
                    {
                        case HttpMethodType.Get:
                            apiFunctions.Add(_kernel.CreateFunctionFromMethod((string msg) =>
                            {
                                try
                                {
                                    Console.WriteLine(msg);
                                    RestClient client = new RestClient();
                                    RestRequest request = new RestRequest(api.Url, Method.Get);
                                    foreach (var header in api.Header.ConvertToString().Split("\n"))
                                    {
                                        var headerArray = header.Split(":");
                                        if (headerArray.Length == 2)
                                        {
                                            request.AddHeader(headerArray[0], headerArray[1]);
                                        }
                                    }
                                    //这里应该还要处理一次参数提取，等后面再迭代
                                    foreach (var query in api.Query.ConvertToString().Split("\n"))
                                    {
                                        var queryArray = query.Split("=");
                                        if (queryArray.Length == 2)
                                        {
                                            request.AddQueryParameter(queryArray[0], queryArray[1]);
                                        }
                                    }
                                    var result = client.Execute(request);
                                    return result.Content;
                                }
                                catch (System.Exception ex)
                                {
                                    return "调用失败：" + ex.Message;
                                }
                            }, api.Name, $"{api.Describe}"));
                            break;

                        case HttpMethodType.Post:
                            apiFunctions.Add(_kernel.CreateFunctionFromMethod((string msg) =>
                            {
                                try
                                {
                                    Console.WriteLine(msg);
                                    RestClient client = new RestClient();
                                    RestRequest request = new RestRequest(api.Url, Method.Post);
                                    foreach (var header in api.Header.ConvertToString().Split("\n"))
                                    {
                                        var headerArray = header.Split(":");
                                        if (headerArray.Length == 2)
                                        {
                                            request.AddHeader(headerArray[0], headerArray[1]);
                                        }
                                    }
                                    //这里应该还要处理一次参数提取，等后面再迭代
                                    request.AddJsonBody(api.JsonBody.ConvertToString());
                                    var result = client.Execute(request);
                                    return result.Content;
                                }
                                catch (System.Exception ex)
                                {
                                    return "调用失败：" + ex.Message;
                                }
                            }, api.Name, $"{api.Describe}"));
                            break;
                    }
                }
            }

            //本地函数插件
            if (!string.IsNullOrWhiteSpace(app.NativeFunctionList))//需要添加判断应用是否开启了本地函数插件
            {
                var nativeIdList = app.NativeFunctionList.Split(",");

                _functionService.SearchMarkedMethods();
                using var scope= _serviceProvider.CreateScope();

                foreach (var func in _functionService.Functions)
                {
                    if (nativeIdList.Contains(func.Key))
                    {
                        var methodInfo = _functionService.MethodInfos[func.Key];
                        var parameters = methodInfo.Parameters.Select(x => new KernelParameterMetadata(x.ParameterName) { ParameterType = x.ParameterType, Description = x.Description });
                        var returnType = new KernelReturnParameterMetadata() { ParameterType = methodInfo.ReturnType.ParameterType, Description = methodInfo.ReturnType.Description };
                        var target = ActivatorUtilities.CreateInstance(scope.ServiceProvider, func.Value.DeclaringType);
                        apiFunctions.Add(_kernel.CreateFunctionFromMethod(func.Value, target, func.Key, methodInfo.Description, parameters, returnType));
                    }
                }
            }
            _kernel.ImportPluginFromFunctions("AntSkFunctions", apiFunctions);
        }

        /// <summary>
        /// 注册默认插件
        /// </summary>
        /// <param name="kernel"></param>
        private void RegisterPluginsWithKernel(Kernel kernel)
        {
            kernel.ImportPluginFromObject(new ConversationSummaryPlugin(), "ConversationSummaryPlugin");
            //kernel.ImportPluginFromObject(new TimePlugin(), "TimePlugin");
            kernel.ImportPluginFromPromptDirectory(Path.Combine(RepoFiles.SamplePluginsPath(), "KMSPlugin"));
        }

        /// <summary>
        /// 会话总结
        /// </summary>
        /// <param name="_kernel"></param>
        /// <param name="questions"></param>
        /// <param name="history"></param>
        /// <returns></returns>
        public async Task<string> HistorySummarize(Kernel _kernel, string questions, string history)
        {
            KernelFunction sunFun = _kernel.Plugins.GetFunction("ConversationSummaryPlugin", "SummarizeConversation");
            var summary = await _kernel.InvokeAsync(sunFun, new() { ["input"] = $"内容是：{history.ToString()} {Environment.NewLine} 请注意用中文总结" });
            string his = summary.GetValue<string>();
            var msg = $"history：{history.ToString()}{Environment.NewLine} user：{questions}"; ;
            return msg;
        }
    }
}
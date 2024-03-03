using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Model;
using AntSK.Domain.Options;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Service
{
    [ServiceDescription(typeof(IKernelService), ServiceLifetime.Scoped)]
    public class KernelService(
        IApis_Repositories _apis_Repositories
        ) : IKernelService
    {
        /// <summary>
        /// 获取kernel实例，依赖注入不好按每个用户去Import不同的插件，所以每次new一个新的kernel
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public Kernel GetKernel(string modelId=null,string apiKey=null)
        {
            var handler = new OpenAIHttpClientHandler();
            var httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            var kernel = Kernel.CreateBuilder()
              .AddOpenAIChatCompletion(
               modelId: modelId!=null? modelId : OpenAIOption.Model,
               apiKey: apiKey!=null? apiKey: OpenAIOption.Key,
               httpClient: httpClient)
               .Build();
            RegisterPluginsWithKernel(kernel);
            return kernel;
        }

        /// <summary>
        /// 根据app配置的插件，导入插件
        /// </summary>
        /// <param name="app"></param>
        /// <param name="_kernel"></param>
        public void ImportFunctionsByApp(Apps app, Kernel _kernel)
        {
            //开启自动插件调用
            var apiIdList = app.ApiFunctionList.Split(",");
            var apiList = _apis_Repositories.GetList(p => apiIdList.Contains(p.Id));
            List<KernelFunction> functions = new List<KernelFunction>();
            var plugin = _kernel.Plugins.FirstOrDefault(p => p.Name == "ApiFunctions");
            {
                foreach (var api in apiList)
                {
                    switch (api.Method)
                    {
                        case HttpMethodType.Get:
                            functions.Add(_kernel.CreateFunctionFromMethod((string msg) =>
                            {
                                try
                                {
                                    Console.WriteLine(msg);
                                    RestClient client = new RestClient();
                                    RestRequest request = new RestRequest(api.Url, Method.Get);
                                    foreach (var header in api.Header.Split("\n"))
                                    {
                                        var headerArray = header.Split(":");
                                        if (headerArray.Length == 2)
                                        {
                                            request.AddHeader(headerArray[0], headerArray[1]);
                                        }
                                    }
                                    //这里应该还要处理一次参数提取，等后面再迭代
                                    foreach (var query in api.Query.Split("\n"))
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
                            functions.Add(_kernel.CreateFunctionFromMethod((string msg) =>
                            {
                                try
                                {
                                    Console.WriteLine(msg);
                                    RestClient client = new RestClient();
                                    RestRequest request = new RestRequest(api.Url, Method.Post);
                                    foreach (var header in api.Header.Split("\n"))
                                    {
                                        var headerArray = header.Split(":");
                                        if (headerArray.Length == 2)
                                        {
                                            request.AddHeader(headerArray[0], headerArray[1]);
                                        }
                                    }
                                    //这里应该还要处理一次参数提取，等后面再迭代
                                    request.AddJsonBody(api.JsonBody);
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
                _kernel.ImportPluginFromFunctions("ApiFunctions", functions);
            }

        }

        /// <summary>
        /// 注册默认插件
        /// </summary>
        /// <param name="kernel"></param>
        void RegisterPluginsWithKernel(Kernel kernel)
        {
            kernel.ImportPluginFromObject(new ConversationSummaryPlugin(), "ConversationSummaryPlugin");
            kernel.ImportPluginFromObject(new TimePlugin(), "TimePlugin");
        }

        /// <summary>
        /// 会话总结
        /// </summary>
        /// <param name="_kernel"></param>
        /// <param name="questions"></param>
        /// <param name="history"></param>
        /// <returns></returns>
        public async Task<string> HistorySummarize(Kernel _kernel,string questions, string history)
        {
            KernelFunction sunFun = _kernel.Plugins.GetFunction("ConversationSummaryPlugin", "SummarizeConversation");
            var summary = await _kernel.InvokeAsync(sunFun, new() { ["input"] = $"内容是：{history.ToString()} {Environment.NewLine} 请注意用中文总结" });
            string his = summary.GetValue<string>();
            var msg = $"历史对话：{his}{Environment.NewLine} 用户问题：{Environment.NewLine}{questions}"; ;
            return msg;
        }
    }
}

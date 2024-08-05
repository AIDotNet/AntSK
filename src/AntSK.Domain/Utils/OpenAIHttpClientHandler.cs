using Serilog;
using System.Text;
using System.Text.RegularExpressions;

namespace AntSK.Domain.Utils
{
    public class OpenAIHttpClientHandler : HttpClientHandler
    {
        private string _endPoint { get; set; }
        public OpenAIHttpClientHandler(string endPoint)
        {
            this._endPoint = endPoint;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            UriBuilder uriBuilder;
            Regex regex = new Regex(@"(https?)://([^/:]+)(:\d+)?/(.*)");
            Match match = regex.Match(_endPoint);
            string guid = Guid.NewGuid().ToString();
            var mediaType = request.Content.Headers.ContentType.MediaType;
            string requestBody = (await request.Content.ReadAsStringAsync()).Unescape();
            var uncaseBody = new StringContent(requestBody, Encoding.UTF8, mediaType);
            request.Content = uncaseBody;

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ConvertToString() != "Production")
            {
                //生产环境根据环境变量可去关闭日志
                //便于调试查看请求prompt
                Log.Information("{Message}", $"【模型服务接口调用-{guid},host:{_endPoint}】:{Environment.NewLine}{requestBody}");
            }

            if (match.Success)
            {
                string xieyi = match.Groups[1].Value;
                string host = match.Groups[2].Value;
                string port = match.Groups[3].Value; // 可选的端口号
                string route = match.Groups[4].Value;
                // 如果port不为空，它将包含冒号，所以你可能需要去除它
                port = string.IsNullOrEmpty(port) ? port : port.Substring(1);
                // 拼接host和端口号
                var hostnew = string.IsNullOrEmpty(port) ? host : $"{host}:{port}";

                switch (request.RequestUri.LocalPath)
                {
                    case "/v1/chat/completions":
                        //替换代理
                        uriBuilder = new UriBuilder(request.RequestUri)
                        {
                            // 这里是你要修改的 URL
                            Scheme = $"{xieyi}://{hostnew}/",
                            Host = host,
                            Path = route + "v1/chat/completions",
                        };
                        if (port.ConvertToInt32() != 0)
                        {
                            uriBuilder.Port = port.ConvertToInt32();
                        }

                        request.RequestUri = uriBuilder.Uri;

                        break;
                    case "/v1/embeddings":
                        uriBuilder = new UriBuilder(request.RequestUri)
                        {
                            // 这里是你要修改的 URL
                            Scheme = $"{xieyi}://{host}/",
                            Host = host,
                            Path = route + "v1/embeddings",
                        };
                        if (port.ConvertToInt32() != 0)
                        {
                            uriBuilder.Port = port.ConvertToInt32();
                        }
                        request.RequestUri = uriBuilder.Uri;
                        break;
                }
            }

            // 接着，调用基类的 SendAsync 方法将你的修改后的请求发出去
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ConvertToString() != "Production")
            {
                string responseContent = requestBody.IsStream() ? response.Content.ReadAsStringAsync().Result : response.Content.ReadAsStringAsync().Result.Unescape();
                Log.Information("{Message}", $"【模型服务接口返回-{guid},host:{_endPoint}】:{Environment.NewLine}{responseContent}");
            }
            return response;
        }
    }


    public class OpenAIHttpClientHandlerUtil
    {
        public static HttpClient GetHttpClient(string endPoint)
        {
            var handler = new OpenAIHttpClientHandler(endPoint.ConvertToString());
            var httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromMinutes(10);
            return httpClient;
        }
    }
}

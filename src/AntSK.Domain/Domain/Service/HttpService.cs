using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Domain.Interface;
using Newtonsoft.Json;
using RestSharp;

namespace AntSK.Domain.Domain.Service
{
    [ServiceDescription(typeof(IHttpService), ServiceLifetime.Scoped)]
    public class HttpService : IHttpService
    {
        public async Task<RestResponse> PostAsync(string url, Object jsonBody)
        {
            RestClient client = new RestClient();
            RestRequest request = new RestRequest(url, Method.Post);
            string josn = JsonConvert.SerializeObject(jsonBody);
            request.AddJsonBody(jsonBody);
            var result = await client.ExecuteAsync(request);
            return result;
        }
    }
}

using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Model;
using AntSK.Domain.Utils;
using Microsoft.KernelMemory;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Service
{
    [ServiceDescription(typeof(IHttpService), ServiceLifetime.Scoped)]
    public class HttpService: IHttpService
    {
        public async Task< RestResponse> PostAsync(string url ,Object jsonBody)
        {
            RestClient client = new RestClient();
            RestRequest request= new RestRequest(url, Method.Post);
            string josn = JsonConvert.SerializeObject(jsonBody);
            request.AddJsonBody(jsonBody);
            var result =await client.ExecuteAsync(request);
            return  result;
        }
    }
}

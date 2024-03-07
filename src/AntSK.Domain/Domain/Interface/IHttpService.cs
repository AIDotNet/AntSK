using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Interface
{
    public interface IHttpService
    {
        Task<RestResponse> PostAsync(string url, Object jsonBody);
    }
}

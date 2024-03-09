using RestSharp;

namespace AntSK.Domain.Domain.Interface
{
    public interface IHttpService
    {
        Task<RestResponse> PostAsync(string url, Object jsonBody);
    }
}

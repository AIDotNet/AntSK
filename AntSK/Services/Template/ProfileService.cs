using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AntSK.Domain.Common.DependencyInjection;
using AntSK.Models;
using ServiceLifetime = AntSK.Domain.Common.DependencyInjection.ServiceLifetime;

namespace AntSK.Services
{
    public interface IProfileService
    {
        Task<BasicProfileDataType> GetBasicAsync();
        Task<AdvancedProfileData> GetAdvancedAsync();
    }
    [ServiceDescription(typeof(IProfileService), ServiceLifetime.Scoped)]
    public class ProfileService : IProfileService
    {
        private readonly HttpClient _httpClient;

        public ProfileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BasicProfileDataType> GetBasicAsync()
        {
            return await _httpClient.GetFromJsonAsync<BasicProfileDataType>("data/basic.json");
        }

        public async Task<AdvancedProfileData> GetAdvancedAsync()
        {
            return await _httpClient.GetFromJsonAsync<AdvancedProfileData>("data/advanced.json");
        }
    }
}
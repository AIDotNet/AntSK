using AntSK.Domain.Common.DependencyInjection;
using AntSK.Models;
using ServiceLifetime = AntSK.Domain.Common.DependencyInjection.ServiceLifetime;

namespace AntSK.Services
{
    public interface IUserService
    {
        Task<CurrentUser> GetCurrentUserAsync();
    }
    [ServiceDescription(typeof(IUserService), ServiceLifetime.Scoped)]
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CurrentUser> GetCurrentUserAsync()
        {
            return await _httpClient.GetFromJsonAsync<CurrentUser>("data/current_user.json");
        }
    }
}
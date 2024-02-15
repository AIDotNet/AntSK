using AntSK.Domain.Options;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace AntSK.Services.Auth
{
    public class AntSKAuthProvider : AuthenticationStateProvider
    {
        private ClaimsIdentity identity = new ClaimsIdentity();

        public async Task<bool> SignIn(string username, string password)
        {
            if (username == LoginOption.User && password == LoginOption.Password)
            {
                // 用户认证成功，创建用户的ClaimsIdentity
                var claims = new[] { new Claim(ClaimTypes.Name, username) };
                identity = new ClaimsIdentity(claims, "AntSK");
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return true;
            }
            else
            {
                // 用户认证失败
                return false;
            }
        }
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }
    }
}

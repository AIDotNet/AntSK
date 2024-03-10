using AntSK.Domain.Options;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using AntSK.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace AntSK.Services.Auth
{
    public class AntSKAuthProvider(
        IUsers_Repositories _users_Repositories,
        ProtectedSessionStorage _protectedSessionStore
        ) : AuthenticationStateProvider
    {
        private ClaimsIdentity identity = new ClaimsIdentity();


        public async Task<bool> SignIn(string username, string password)
        {

            var user = _users_Repositories.GetFirst(p => p.No == username);
            if (username == LoginOption.User && password == LoginOption.Password)
            {
                // 管理员认证成功，创建用户的ClaimsIdentity
                var claims = new[] { new Claim(ClaimTypes.Name, username) };
                identity = new ClaimsIdentity(claims, "AntSKAdmin");
                await _protectedSessionStore.SetAsync("UserSession", new UserSession() { UserName = username, Role = "AntSKAdmin" });
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return true;
            }
            else
            {
                if (user.IsNull())
                {
                    return false;
                }
                if (!PasswordUtil.VerifyPassword(password, user.Password))
                {
                    return false;
                }
                // 用户认证成功，创建用户的ClaimsIdentity
                var claims = new[] { new Claim(ClaimTypes.Name, username) };
                identity = new ClaimsIdentity(claims, "AntSKUser");
                await _protectedSessionStore.SetAsync("UserSession", new UserSession() { UserName = username, Role = "AntSKUser" });
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return true;
            }
        }

        public ClaimsPrincipal GetCurrentUser()
        {
            var user = new ClaimsPrincipal(identity);
            return user;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var userSessionStorageResult = await _protectedSessionStore.GetAsync<UserSession>("UserSession");
            var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;
            if (userSession.IsNotNull())
            {
                var claims = new[] { new Claim(ClaimTypes.Name, userSession.UserName) };
                identity = new ClaimsIdentity(claims, "AntSKUser");
            }
            var user = new ClaimsPrincipal(identity);
            return await Task.FromResult(new AuthenticationState(user));
        }
    }
}

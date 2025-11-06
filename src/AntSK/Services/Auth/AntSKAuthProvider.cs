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
        IUserRoles_Repositories _userRoles_Repositories,
        IRoles_Repositories _roles_Repositories,
        IRolePermissions_Repositories _rolePermissions_Repositories,
        IPermissions_Repositories _permissions_Repositories,
        ProtectedSessionStorage _protectedSessionStore
        ) : AuthenticationStateProvider
    {
        private ClaimsIdentity identity = new ClaimsIdentity();


        public async Task<bool> SignIn(string username, string password)
        {

            var user = _users_Repositories.GetFirst(p => p.No == username);
            if (username == LoginOption.User && password == LoginOption.Password)
            {
                string AdminRole = "AntSKAdmin";
                // 管理员认证成功，创建用户的ClaimsIdentity
                var claims = new[] {    
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, AdminRole)
                };
                identity = new ClaimsIdentity(claims, AdminRole);
                await _protectedSessionStore.SetAsync("UserSession", new UserSession() 
                { 
                    UserName = username, 
                    Role = AdminRole,
                    Roles = new List<string> { AdminRole },
                    Permissions = new List<string>()
                });
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

                // 获取用户的角色和权限
                var userRoles = _userRoles_Repositories.GetList(p => p.UserId == user.Id);
                var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
                var roles = _roles_Repositories.GetList(r => roleIds.Contains(r.Id) && r.IsEnabled);
                
                // 获取角色的权限
                var rolePermissions = _rolePermissions_Repositories.GetList(rp => roleIds.Contains(rp.RoleId));
                var permissionIds = rolePermissions.Select(rp => rp.PermissionId).Distinct().ToList();
                var permissions = _permissions_Repositories.GetList(p => permissionIds.Contains(p.Id));

                // 如果没有角色，使用默认角色
                string defaultRole = "AntSKUser";
                var roleList = roles.Any() ? roles.Select(r => r.Code).ToList() : new List<string> { defaultRole };
                var permissionList = permissions.Select(p => p.Code).ToList();

                // 用户认证成功，创建用户的ClaimsIdentity
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)
                };

                // 添加所有角色到Claims
                foreach (var role in roleList)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                identity = new ClaimsIdentity(claims, roleList.FirstOrDefault() ?? defaultRole);
                await _protectedSessionStore.SetAsync("UserSession", new UserSession() 
                { 
                    UserName = username, 
                    Role = roleList.FirstOrDefault() ?? defaultRole,
                    Roles = roleList,
                    Permissions = permissionList
                });
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
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.UserName)
                };

                // 添加所有角色到Claims
                if (userSession.Roles != null && userSession.Roles.Any())
                {
                    foreach (var role in userSession.Roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }
                else
                {
                    // 向后兼容，使用单个Role
                    claims.Add(new Claim(ClaimTypes.Role, userSession.Role));
                }

                identity = new ClaimsIdentity(claims, userSession.Role);
            }
            var user = new ClaimsPrincipal(identity);
            return await Task.FromResult(new AuthenticationState(user));
        }
    }
}

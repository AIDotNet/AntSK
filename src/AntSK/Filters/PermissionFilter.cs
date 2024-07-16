using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using AntSK.Filters;

namespace AntSK.Midware
{
    public class PermissionFilter : IAsyncAuthorizationFilter
    {
        private ConfigurationManager _configuration;
        public PermissionFilter(ConfigurationManager configuration)
        {
            _configuration = configuration;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // 获取操作描述符
            var actionDescriptor = context.ActionDescriptor;
            // 检查是否有Authorize特性
            bool isAuthorized = actionDescriptor.EndpointMetadata
                .OfType<TokenCheckAttribute>()
                .Any();
            if (isAuthorized)
            {
                var authorization = context.HttpContext.Request.Headers.Authorization;
                var apiToken = _configuration.GetSection("ApiToken").Value;
                if (apiToken != authorization)
                {
                    context.HttpContext.Response.StatusCode = 401;
                    context.Result = new ContentResult() { Content = "没有权限" };
                    return;
                }
            }

            // 如果有Authorize特性，继续执行操作
            await Task.CompletedTask;
        }
    }
}

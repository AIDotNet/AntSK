using AntSK.Domain.Common;
using Microsoft.AspNetCore.Authorization;

namespace AntSK.Filters
{
    public class OpenApiAuthorizationHandler : AuthorizationHandler<OpenApiRequirement>
    {
        private string _expectedToken = string.Empty;

        public OpenApiAuthorizationHandler(IConfiguration configuration)
        {
            _expectedToken = configuration["ApiToken"];
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OpenApiRequirement requirement)
        {
            var httpContext = context.Resource as HttpContext;
            if (httpContext != null)
            {
                var token = httpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(token))
                {
                    if (IsValidToken(token))
                    {
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }
                }
            }
            context.Fail();
            throw new AntSkUnAuthorizeException(401, "对不起， 您没有权限访问");
        }
        private bool IsValidToken(string token)
        {
            return _expectedToken == token;
        }
    }

    public class OpenApiRequirement : IAuthorizationRequirement
    {

    }
}

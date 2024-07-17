using AntSK.Domain.Common;
using System.Net;

namespace AntSK.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (ex is AntSkUnAuthorizeException)
                {
                    // 设置 HTTP 状态码
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync(ex.Message);
                    return;
                }
                _logger.LogError($"Unhandled exception: {ex}");
                // 设置 HTTP 状态码
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                // 返回错误信息，实际生产中可能需要返回更友好的错误页面或信息
                await context.Response.WriteAsync("An error occurred while processing your request.");
            }
        }
    }
}

using AntSK.BackgroundTask;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AntSK.Domain.Domain.Other
{
    public class BackGroundTaskHandler : IBackgroundTaskHandler<ImportKMSTaskReq>
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BackGroundTaskHandler> _logger;

        public BackGroundTaskHandler(IServiceScopeFactory scopeFactory, ILogger<BackGroundTaskHandler> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        public async Task ExecuteAsync(ImportKMSTaskReq item)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                _logger.LogInformation("ExecuteAsync.开始执行后台任务");
                var importKMSService = scope.ServiceProvider.GetRequiredService<IImportKMSService>();
                //不能使用异步
                importKMSService.ImportKMSTask(item);
                _logger.LogInformation("ExecuteAsync.后台任务执行完成");
            }

        }

        public Task OnFailed()
        {
            return Task.CompletedTask;

        }

        public Task OnSuccess()
        {
            return Task.CompletedTask;
        }

    }
}

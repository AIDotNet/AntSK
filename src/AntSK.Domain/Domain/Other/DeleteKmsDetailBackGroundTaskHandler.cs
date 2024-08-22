using AntSK.BackgroundTask;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AntSK.Domain.Domain.Other
{
    public class DeleteKmsDetailBackGroundTaskHandler : IBackgroundTaskHandler<DeleteKmsDetailReq>
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DeleteKmsDetailBackGroundTaskHandler> _logger;

        public DeleteKmsDetailBackGroundTaskHandler(IServiceScopeFactory scopeFactory, ILogger<DeleteKmsDetailBackGroundTaskHandler> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        public Task ExecuteAsync(DeleteKmsDetailReq item)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                _logger.LogInformation("ExecuteAsync.开始 文档删除 任务");
                var kMService = scope.ServiceProvider.GetRequiredService<IKMService>();

                var _memory = kMService.GetMemoryByKMS(item.KmsId);
                if (_memory != null)
                {
                    try
                    {
                        _memory.DeleteDocumentAsync(index: "kms", documentId: item.DocumentId);
                    }
                    catch (FileNotFoundException ex)
                    {
                        _logger.LogError(ex, "删除KMS文档异常,未找到文件 {id}", item.DocumentId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "删除KMS文档异常 {id}", item.DocumentId);
                    }
                }
                _logger.LogInformation("ExecuteAsync.完成 文档删除 任务");
            }
            return Task.CompletedTask;
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

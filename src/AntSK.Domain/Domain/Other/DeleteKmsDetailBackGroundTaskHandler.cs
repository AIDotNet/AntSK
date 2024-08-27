using AntSK.BackgroundTask;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Domain.Model;
using AntSK.Domain.Repositories;
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
        public async Task ExecuteAsync(DeleteKmsDetailReq item)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                _logger.LogInformation("ExecuteAsync.开始 文档删除 任务");
                var kMService = scope.ServiceProvider.GetRequiredService<IKMService>();
                var taskBroker = scope.ServiceProvider.GetRequiredService<BackgroundTaskBroker<ImportKMSTaskReq>>();
                var _kmsDetails_Repositories = scope.ServiceProvider.GetRequiredService<IKmsDetails_Repositories>();
                var _memory = kMService.GetMemoryByKMS(item.KmsId);
                if (_memory != null)
                {
                    try
                    {
                        await _memory.DeleteDocumentAsync(index: "kms", documentId: item.DocumentId);
                    }
                    catch (FileNotFoundException ex)
                    {
                        _logger.LogError(ex, "删除KMS文档异常,未找到文件 {id}", item.DocumentId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "删除KMS文档异常 {id}", item.DocumentId);
                    }
                    if (item.ReCut)
                    {
                        var detail = _kmsDetails_Repositories.GetById(item.DocumentId);
                        if (detail == null)
                        {
                            return;
                        }

                        var req = new ImportKMSTaskReq();
                        req.ImportType = GetImportType(detail.Type);
                        req.KmsId = item.KmsId;
                        req.Url = detail.Url;
                        req.FileName = detail.FileName;
                        req.FilePath = detail.FileGuidName;
                        req.KmsDetail = detail;
                        req.IsQA = false;
                        taskBroker.QueueWorkItem(req);
                    }
                }
                _logger.LogInformation("ExecuteAsync.完成 文档删除 任务");
            }
            return;
        }
        private ImportType GetImportType(string type)
        {
            switch (type)
            {
                case "url":
                    return ImportType.Url;
                case "file":
                    return ImportType.File;
                case "text":
                    return ImportType.Text;
                case "excel":
                    return ImportType.Excel;
                default:
                    throw new NotImplementedException();
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

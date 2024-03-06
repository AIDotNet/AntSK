using AntSK.Domain.BackgroundTask;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Service
{
    public class ImportKMSTaskHandler : IBackgroundTaskHandler<ImportKMSTaskReq>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ImportKMSTaskHandler(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public async Task ExecuteAsync(ImportKMSTaskReq item)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                Console.WriteLine("ImportKMSTaskHandler.开始执行任务");
                var  importKMSService = scope.ServiceProvider.GetRequiredService<IImportKMSService>();
                await importKMSService.ImportKMSTask(item);
                Console.WriteLine("ImportKMSTaskHandler.任务执行完成");
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

using AntSK.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static AntSK.Pages.KmsPage.KmsDetail;
using System;
using AntSK.Domain.Repositories;
using AntSK.Domain.Domain.Interface;
using Microsoft.KernelMemory.Configuration;
using AntSK.Domain.Model.Enum;
using AntSK.Domain.Map;
using AntSK.BackgroundTask;

namespace AntSK.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_taskBroker"></param>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KMSController : ControllerBase
    {
        private readonly IKmsDetails_Repositories _kmsDetails_Repositories;
        private readonly IKMService _iKMService;
        private readonly BackgroundTaskBroker<ImportKMSTaskReq> _taskBroker;
        public KMSController(
            IKmsDetails_Repositories kmsDetails_Repositories,
            IKMService iKMService,
            BackgroundTaskBroker<ImportKMSTaskReq> taskBroker
            ) 
        {
            _kmsDetails_Repositories = kmsDetails_Repositories;
            _iKMService = iKMService;
            _taskBroker = taskBroker;
        }
        [HttpPost]
        public async Task<IActionResult> ImportKMSTask(ImportKMSTaskDTO model) 
        {
            Console.WriteLine("api/kms/ImportKMSTask  开始");
            ImportKMSTaskReq req = model.ToDTO<ImportKMSTaskReq>();
            KmsDetails detail = new KmsDetails()
            {
                Id = Guid.NewGuid().ToString(),
                KmsId = req.KmsId,
                CreateTime = DateTime.Now,
                Status = ImportKmsStatus.Loadding,
                Type = model.ImportType.ToString().ToLower()
            };

            _kmsDetails_Repositories.Insert(detail);
            req.KmsDetail = detail;
            _taskBroker.QueueWorkItem(req);
            Console.WriteLine("api/kms/ImportKMSTask  结束");
            return Ok();
        }
    }
}

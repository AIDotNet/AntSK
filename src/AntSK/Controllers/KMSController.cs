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
    public class KMSController(
        IKmsDetails_Repositories _kmsDetails_Repositories,
        IKMService _iKMService,
        BackgroundTaskBroker<ImportKMSTaskReq> _taskBroker
        ) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> ImportKMSTask(ImportKMSTaskDTO model) 
        {
            Console.WriteLine("api/kms/ImportKMSTask  开始");
            ImportKMSTaskReq req = new ImportKMSTaskReq()
            {
                FileName = model.FileName,
                FilePath = model.FilePath,
                Text = model.Text,
                ImportType = model.ImportType,
                KmsId = model.KmsId,
                Url = model.Url
            };
            KmsDetails detail = new KmsDetails()
            {
                Id=Guid.NewGuid().ToString(),
                KmsId=req.KmsId,
                CreateTime=DateTime.Now,
                Status=ImportKmsStatus.Loadding,
                Type= model.ImportType.ToString().ToLower()
            };
            
            _kmsDetails_Repositories.Insert(detail);
            req.KmsDetail = detail;
            _taskBroker.QueueWorkItem(req);
            Console.WriteLine("api/kms/ImportKMSTask  结束");
            return Ok();
        }
    }
}

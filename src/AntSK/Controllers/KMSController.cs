using AntSK.BackgroundTask;
using AntSK.Domain.Common.Map;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Domain.Model;
using AntSK.Domain.Domain.Model.Enum;
using AntSK.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AntSK.Controllers
{
    /// <summary>
    /// KMSController
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KMSController : ControllerBase
    {
        private readonly IKmsDetails_Repositories _kmsDetailsRepositories;
        private readonly BackgroundTaskBroker<ImportKMSTaskReq> _taskBroker;

        public KMSController(
            IKmsDetails_Repositories kmsDetailsRepositories,
            BackgroundTaskBroker<ImportKMSTaskReq> taskBroker
        )
        {
            _kmsDetailsRepositories = kmsDetailsRepositories;
            _taskBroker = taskBroker;
        }

        /// <summary>
        /// 导入任务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ImportKMSTask(ImportKMSTaskDTO model)
        {
            ImportKMSTaskReq req = model.ToDTO<ImportKMSTaskReq>();
            KmsDetails detail = new KmsDetails()
            {
                Id = Guid.NewGuid().ToString(),
                KmsId = req.KmsId,
                CreateTime = DateTime.Now,
                Status = ImportKmsStatus.Loadding,
                Type = model.ImportType.ToString().ToLower()
            };

            await _kmsDetailsRepositories.InsertAsync(detail);
            req.KmsDetail = detail;
            _taskBroker.QueueWorkItem(req);
            return Ok();
        }
    }
}
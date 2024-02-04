using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AntSK.Domain.Repositories;

namespace AntSK.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InitController : ControllerBase
    {
        private readonly IApps_Repositories _repository;

        public InitController(IApps_Repositories repository)
        {
            _repository = repository;
        }
        /// <summary>
        /// 初始化DB 和表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult InitTable()
        {
            _repository.GetDB().DbMaintenance.CreateDatabase();
            _repository.GetDB().CodeFirst.InitTables(typeof(Apps));
            _repository.GetDB().CodeFirst.InitTables(typeof(Kmss));
            _repository.GetDB().CodeFirst.InitTables(typeof(KmsDetails));
            return Ok();
        }
    }
}

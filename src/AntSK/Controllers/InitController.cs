using AntSK.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

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
            _repository.GetDB().CodeFirst.InitTables(typeof(Users));
            _repository.GetDB().CodeFirst.InitTables(typeof(Apis));
            _repository.GetDB().CodeFirst.InitTables(typeof(AIModels));
            //创建vector插件如果数据库没有则需要提供支持向量的数据库
            _repository.GetDB().Ado.ExecuteCommandAsync($"CREATE EXTENSION IF NOT EXISTS vector;");
            return Ok();
        }
    }
}

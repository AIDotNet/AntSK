using AntSK.Domain.Repositories;
using AntSK.Models.Dto;
using AntSK.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AntSK.Domain.Domain.Model.Enum;

namespace AntSK.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Policy = "openapi")]
    public class AppController(IApps_Repositories apps_Repositories) : ControllerBase
    {
        /// <summary>
        /// 获取应用列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ExecuteResult<AppListDto>> GetListAsync()
        {
            var list = await apps_Repositories.GetListAsync(x => x.Type == AppType.kms.ToString());
            var r = new AppListDto();
            r.Items = list.Select(x => new AppDto { Id = x.Id, Name = x.Name }).ToList();
            r.TotalCount = list.Count;
            return ExecuteResult<AppListDto>.Success(r);
        }

        /// <summary>
        /// 获取应用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ExecuteResult<AppDto>> GetAsync([FromBody] string id)
        {
            var item = await apps_Repositories.GetFirstAsync(x => x.Id == id);
            if (item == null)
            {
                return ExecuteResult<AppDto>.Error("应用不存在!");
            }
            return ExecuteResult<AppDto>.Success(new AppDto { Id = item.Id, Name = item.Name, SecretKey = item.SecretKey });
        }
    }
}

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
        [HttpPost]
        public async Task<ExecuteResult<AppListDto>> GetListAsync()
        {
            var list = await apps_Repositories.GetListAsync(x => x.Type == AppType.kms.ToString());
            var r = new AppListDto();
            r.Items = list.Select(x => new AppDto { Id = x.Id, Name = x.Name, SecretKey = x.SecretKey }).ToList();
            r.TotalCount = list.Count;
            return ExecuteResult<AppListDto>.Success(r);
        }
    }
}

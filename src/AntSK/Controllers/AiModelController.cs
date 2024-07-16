using AntSK.Domain.Repositories;
using AntSK.Filters;
using AntSK.Models;
using AntSK.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AntSK.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [TokenCheck]
    public class AiModelController(IAIModels_Repositories aIModels_Repositories) : ControllerBase
    {
        [HttpPost]
        public async Task<ExecuteResult<AiModelListDto>> GetListAsync()
        {
            var list = await aIModels_Repositories.GetListAsync();
            var r = new AiModelListDto();
            r.Items = list.Select(x => new AiModelDto { Id = x.Id, ModelName = x.ModelName, ModelDescription = x.ModelDescription }).ToList();
            r.TotalCount = list.Count;
            return ExecuteResult<AiModelListDto>.Success(r);
        }
    }
}

using AntSK.Domain.Repositories;
using AntSK.Filters;
using AntSK.Models;
using AntSK.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AntSK.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="kmss_Repositories"></param>
    /// <param name="configuration"></param>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [TokenCheck]
    public class KMSExtendController(IKmss_Repositories kmss_Repositories, IConfiguration configuration) : ControllerBase
    {
        /// <summary>
        /// 保存知识库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ExecuteResult<KmsReturnDto>> SaveAsync([FromBody] KmsEditDto model)
        {
            return await (string.IsNullOrWhiteSpace(model.Id) ? AddAsync(model) : UpdateAsync(model));
        }


        async Task<ExecuteResult<KmsReturnDto>> AddAsync([FromBody] KmsEditDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return ExecuteResult<KmsReturnDto>.Error("知识库名称不能为空");
            }
            if (await kmss_Repositories.IsAnyAsync(p => p.Name == model.Name))
            {
                return ExecuteResult<KmsReturnDto>.Error("名称已存在！");
            }
            var _kmsModel = new Kmss();
            _kmsModel.Id = Guid.NewGuid().ToString();
            _kmsModel.ChatModelID = configuration.GetSection("DefaultModel:Chat").Value;
            _kmsModel.EmbeddingModelID = configuration.GetSection("DefaultModel:Embedding").Value;
            var result = await kmss_Repositories.InsertAsync(_kmsModel);
            return result ?
                ExecuteResult<KmsReturnDto>.Success(new KmsReturnDto { Id = _kmsModel.Id, Name = _kmsModel.Name }) :
                ExecuteResult<KmsReturnDto>.Error("保存失败！");
        }
        async Task<ExecuteResult<KmsReturnDto>> UpdateAsync([FromBody] KmsEditDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return ExecuteResult<KmsReturnDto>.Error("知识库名称不能为空");
            }
            if (await kmss_Repositories.IsAnyAsync(p => p.Name == model.Name && p.Id != model.Id))
            {
                return ExecuteResult<KmsReturnDto>.Error("名称已存在！");
            }
            var _kmsModel = await kmss_Repositories.GetFirstAsync(p => p.Id == model.Id);
            if (_kmsModel == null)
            {
                return ExecuteResult<KmsReturnDto>.Error("知识库不存在");
            }
            _kmsModel.Name = model.Name;
            var result = await kmss_Repositories.UpdateAsync(_kmsModel);
            return result ?
                ExecuteResult<KmsReturnDto>.Success(new KmsReturnDto { Id = _kmsModel.Id, Name = _kmsModel.Name }) :
                ExecuteResult<KmsReturnDto>.Error("保存失败！");
        }
    }
}

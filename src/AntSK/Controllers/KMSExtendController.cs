using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Domain.Model;
using AntSK.Domain.Domain.Model.Enum;
using AntSK.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using AntSK.Filters;
using AntSK.Models;
using AntSK.Models.Dto;
using AntSK.BackgroundTask;
using AntSK.Domain.Common.Map;

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
    public class KMSExtendController(
        IAIModels_Repositories aIModels_Repositories,
        IKmss_Repositories kmss_Repositories,
        IKmsDetails_Repositories kmsDetails_Repositories,
        IKMService kMService,
        BackgroundTaskBroker<ImportKMSTaskReq> taskBroker,
        IConfiguration configuration) : ControllerBase
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

        /// <summary>
        /// 知识库列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ExecuteResult<List<KmsReturnDto>>> GetListAsync()
        {
            return ExecuteResult<List<KmsReturnDto>>
                .Success((await kmss_Repositories.GetListAsync())
                .Select(x => new KmsReturnDto { Id = x.Id, Name = x.Name })
                .ToList());
        }

        /// <summary>
        /// 删除知识库
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ExecuteResult> DeleteAsync([FromQuery] string id)
        {
            var _memory = kMService.GetMemoryByKMS(id);
            var detailList = kmsDetails_Repositories.GetList(p => p.KmsId == id);
            if (detailList != null)
            {
                foreach (var detail in detailList)
                {
                    var flag = await kmsDetails_Repositories.DeleteAsync(detail.Id);
                    if (flag)
                    {
                        if (_memory != null)
                        {
                            await _memory.DeleteDocumentAsync(index: "kms", documentId: detail.Id);
                        }

                    }
                }
            }
            var result = await kmss_Repositories.DeleteAsync(id);
            return result ? ExecuteResult.Success("删除成功") : ExecuteResult.Error("删除失败");
        }

        /// <summary>
        /// 导入任务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ExecuteResult<KmsDetailsDto>> ImportKMSTask([FromBody] ImportKMSTaskDTO model)
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

            var result = await kmsDetails_Repositories.InsertAsync(detail);
            if (!result)
            {
                return ExecuteResult<KmsDetailsDto>.Error("导入失败");
            }
            req.KmsDetail = detail;
            req.IsQA = model.IsQA;
            taskBroker.QueueWorkItem(req);
            return ExecuteResult<KmsDetailsDto>.Success(detail.ToDTO<KmsDetailsDto>());
        }
        /// <summary>
        /// 获取知识详情
        /// </summary>
        /// <param name="detailId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ExecuteResult<KmsDetailsDto>> GetDetail([FromQuery] string detailId)
        {
            var model = await kmsDetails_Repositories.GetByIdAsync(detailId);
            if (model == null)
                return ExecuteResult<KmsDetailsDto>.Error("未找到详情");
            return ExecuteResult<KmsDetailsDto>.Success(model.ToDTO<KmsDetailsDto>());
        }
        /// <summary>
        /// 删除知识
        /// </summary>
        /// <param name="detailId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ExecuteResult> DeleteDetail([FromQuery] string detailId)
        {
            var model = await kmsDetails_Repositories.GetByIdAsync(detailId);
            if (model == null)
                return ExecuteResult.Success("删除成功");
            if (model.Status == ImportKmsStatus.Loadding)
            {
                return ExecuteResult.Error("导入中不能删除");
            }
            var result = await kmsDetails_Repositories.DeleteAsync(detailId);
            return result ? ExecuteResult.Success("删除成功") : ExecuteResult.Error("删除失败");
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
            var chatDefault = aIModels_Repositories.GetFirst(x => x.Defalut == true && x.AIModelType == AIModelType.Chat);
            if (chatDefault == null)
            {
                return ExecuteResult<KmsReturnDto>.Error("默认会话模型未配置，无法创建知识库");
            }
            var embeddingDefault = aIModels_Repositories.GetFirst(x => x.Defalut == true && x.AIModelType == AIModelType.Embedding);
            if (embeddingDefault == null)
            {
                return ExecuteResult<KmsReturnDto>.Error("默认向量模型未配置，无法创建知识库");
            }
            var _kmsModel = new Kmss();
            _kmsModel.Icon = "chrome";
            _kmsModel.Name = model.Name;
            _kmsModel.Describe = model.Name;
            _kmsModel.Id = Guid.NewGuid().ToString();
            //_kmsModel.ChatModelID = configuration.GetSection("DefaultModel:Chat").Value;
            //_kmsModel.EmbeddingModelID = configuration.GetSection("DefaultModel:Embedding").Value;
            _kmsModel.ChatModelID = chatDefault.Id;
            _kmsModel.EmbeddingModelID = embeddingDefault.Id;
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
            if (_kmsModel.Name == model.Name)
            {
                return ExecuteResult<KmsReturnDto>.Success(new KmsReturnDto { Id = _kmsModel.Id, Name = _kmsModel.Name });
            }
            _kmsModel.Name = model.Name;
            var result = await kmss_Repositories.UpdateAsync(_kmsModel);
            return result ?
                ExecuteResult<KmsReturnDto>.Success(new KmsReturnDto { Id = _kmsModel.Id, Name = _kmsModel.Name }) :
                ExecuteResult<KmsReturnDto>.Error("保存失败！");
        }
    }
}

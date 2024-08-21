using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Domain.Model;
using AntSK.Domain.Domain.Model.Enum;
using AntSK.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using AntSK.Models;
using AntSK.Models.Dto;
using AntSK.BackgroundTask;
using AntSK.Domain.Common.Map;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace AntSK.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="kmss_Repositories"></param>
    /// <param name="configuration"></param>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Policy = "openapi")]
    public class KMSExtendController(
        IAIModels_Repositories aIModels_Repositories,
        IKmss_Repositories kmss_Repositories,
        IKmsDetails_Repositories kmsDetails_Repositories,
        IKMService kMService,
        BackgroundTaskBroker<ImportKMSTaskReq> taskBroker,
        IConfiguration configuration, ILogger<KMSExtendController> logger) : ControllerBase
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
        /// 批量导入任务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ExecuteResult<List<KmsDetailsDto>>> BatchImportKMSTask([FromBody] List<ImportKMSTaskDTO> model)
        {
            var list = new List<KmsDetails>();
            foreach (var item in model)
            {
                KmsDetails detail = new KmsDetails()
                {
                    Id = Guid.NewGuid().ToString(),
                    KmsId = item.KmsId,
                    CreateTime = DateTime.Now,
                    Status = ImportKmsStatus.Loadding,
                    FileName = item.FileName,
                    Url = item.Url,
                    Type = item.ImportType.ToString().ToLower()
                };
                list.Add(detail);
            }
            var result = await kmsDetails_Repositories.InsertRangeAsync(list);
            if (!result)
            {
                return ExecuteResult<List<KmsDetailsDto>>.Error("批量导入失败");
            }
            for (var i = 0; i < model.Count; i++)
            {
                var req = model[i].ToDTO<ImportKMSTaskReq>();
                req.KmsDetail = list[i];
                req.IsQA = model[i].IsQA;
                taskBroker.QueueWorkItem(req);
            }
            return ExecuteResult<List<KmsDetailsDto>>.Success(list.Select(x => x.ToDTO<KmsDetailsDto>()).ToList());
        }


        /// <summary>
        /// 获取知识详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ExecuteResult<KmsDetailsDto>> GetDetail([FromQuery] string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return ExecuteResult<KmsDetailsDto>.Error("未找到详情");
            var model = await kmsDetails_Repositories.GetByIdAsync(id);
            if (model == null)
                return ExecuteResult<KmsDetailsDto>.Error("未找到详情");
            return ExecuteResult<KmsDetailsDto>.Success(model.ToDTO<KmsDetailsDto>());
        }
        /// <summary>
        /// 获取知识详情
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ExecuteResult<List<KmsDetailsDto>>> GetDetails([FromBody] string[] ids)
        {
            if (ids == null || !ids.Any()) return ExecuteResult<List<KmsDetailsDto>>.Error("未找到详情");
            var list = await kmsDetails_Repositories.GetListAsync(x => ids.Contains(x.Id));
            if (list == null)
                return ExecuteResult<List<KmsDetailsDto>>.Error("未找到详情");
            return ExecuteResult<List<KmsDetailsDto>>.Success(list.Select(x => x.ToDTO<KmsDetailsDto>()).ToList());
        }


        /// <summary>
        /// 删除知识详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ExecuteResult> DeleteDetail([FromQuery] string id)
        {
            var model = await kmsDetails_Repositories.GetByIdAsync(id);
            if (model == null)
                return ExecuteResult.Success("删除成功");
            var result = await kmsDetails_Repositories.DeleteAsync(id);
            if (result)
            {
                var _memory = kMService.GetMemoryByKMS(model.KmsId);
                if (_memory != null)
                {
                    try
                    {
                        await _memory.DeleteDocumentAsync(index: "kms", documentId: id);
                    }
                    catch (FileNotFoundException ex)
                    {
                        logger.LogError(ex, "删除KMS文档异常,未找到文件 {id}", id);
                        return ExecuteResult.Success("删除成功");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "删除KMS文档异常 {id}", id);
                        return ExecuteResult.Error("删除KMS文档异常");
                    }
                }

            }
            return result ? ExecuteResult.Success("删除成功") : ExecuteResult.Error("删除失败");
        }
        /// <summary>
        /// 移动到知识库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ExecuteResult> MoveTo([FromBody] KmsMoveDto model)
        {
            if (string.IsNullOrWhiteSpace(model.FromId) || string.IsNullOrWhiteSpace(model.ToId))
                return ExecuteResult.Error("fromId或toId不能为空");
            var list = await kmsDetails_Repositories.GetListAsync(x => x.KmsId == model.FromId);
            int success = 0;
            int fail = 0;
            if (list != null && list.Any())
            {
                foreach (var item in list)
                {
                    item.KmsId = model.ToId;
                    var r = await kmsDetails_Repositories.UpdateAsync(item);
                    if (r)
                    {
                        logger.LogDebug("转移成功 {id}", item.Id);
                        success++;
                    }
                    else
                    {
                        logger.LogDebug("转移失败 {id}", item.Id);
                        fail++;
                    }
                }
            }
            return ExecuteResult.Success($"转移结果,成功数:{success} 失败数:{fail}");
        }
        /// <summary>
        /// 详情移动到知识库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ExecuteResult> DetailMoveTo([FromBody] DetailMoveToDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Id) || string.IsNullOrWhiteSpace(model.KmsId))
                return ExecuteResult.Error("Id或KmsId不能为空");
            var detail = await kmsDetails_Repositories.GetFirstAsync(x => x.Id == model.Id);
            if (detail == null)
            {
                return ExecuteResult.Error("知识文件不存在");
            }
            if (detail.KmsId == model.KmsId)
            {
                return ExecuteResult.Success("转移成功");
            }
            detail.KmsId = model.KmsId;
            var result = await kmsDetails_Repositories.UpdateAsync(detail);
            return result ? ExecuteResult.Success("转移成功") : ExecuteResult.Error("转移失败");
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

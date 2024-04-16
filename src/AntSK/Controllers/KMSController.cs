using AntDesign;
using AntSK.BackgroundTask;
using AntSK.Domain.Common.Map;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Domain.Model;
using AntSK.Domain.Domain.Model.Enum;
using AntSK.Domain.Domain.Service;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Text;
using System.Text.RegularExpressions;

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
        private readonly IKernelService _kernelService;

        public KMSController(
            IKmsDetails_Repositories kmsDetailsRepositories,
            BackgroundTaskBroker<ImportKMSTaskReq> taskBroker,
            IKernelService kernelService
        )
        {
            _kmsDetailsRepositories = kmsDetailsRepositories;
            _taskBroker = taskBroker;
            _kernelService = kernelService;
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
            req.IsQA=model.IsQA;
            _taskBroker.QueueWorkItem(req);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> QA(QAModel model)
        {
            var kernel = _kernelService.GetKernelByAIModelID(model.ChatModelId);
            var lines = TextChunker.SplitPlainTextLines(model.Context, 299);
            var paragraphs = TextChunker.SplitPlainTextParagraphs(lines, 4000);
            KernelFunction jsonFun = kernel.Plugins.GetFunction("KMSPlugin", "QA");

            List<string> qaList = new List<string>();
            foreach (var para in paragraphs)
            {
                var qaresult = await kernel.InvokeAsync(function: jsonFun, new KernelArguments() { ["input"] = para });
                var qaListStr = qaresult.GetValue<string>().ConvertToString();

                string pattern = @"Q\d+:.*?A\d+:.*?(?=(Q\d+:|$))";
                RegexOptions options = RegexOptions.Singleline;

                foreach (Match match in Regex.Matches(qaListStr, pattern, options))
                {
                    qaList.Add(match.Value.Trim()); // Trim用于删除可能的首尾空格
                }
            }
            return Ok(qaList);
        }
    }
}
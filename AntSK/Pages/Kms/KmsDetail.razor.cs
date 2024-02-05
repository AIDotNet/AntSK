using AntDesign;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using AntSK.Models;
using AntSK.Services;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.KernelMemory;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace AntSK.Pages.Kms
{
    public partial class KmsDetail
    {
        [Parameter]
        public string KmsId { get; set; }

        private readonly KmsDetails _model = new KmsDetails();

        bool _urlVisible = false;
        bool _urlConfirmLoading = false;

        private Form<UrlModel> _urlForm;
        private UrlModel urlModel = new UrlModel();

        private readonly IDictionary<string, ProgressStatus> _pStatus = new Dictionary<string, ProgressStatus>
        {
            {"active", ProgressStatus.Active},
            {"exception", ProgressStatus.Exception},
            {"normal", ProgressStatus.Normal},
            {"success", ProgressStatus.Success}
        };

        private List<KmsDetails> _data =new List<KmsDetails>();

        [Inject]
        protected IKmsDetails_Repositories _kmsDetails_Repositories { get; set; }
        [Inject]
        protected MemoryServerless _memory { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _data =await _kmsDetails_Repositories.GetListAsync(p => p.KmsId == KmsId);
        }
        /// <summary>
        /// 根据文档ID获取文档
        /// </summary>
        /// <param name="fileid"></param>
        /// <returns></returns>
        private async Task<List<string>> GetDocumentByFileID(string fileid)
        {
            var memories = await _memory.ListIndexesAsync();
            var memoryDbs = _memory.Orchestrator.GetMemoryDbs();
            List<string> docTextList = new List<string>();

            foreach (var memoryIndex in memories)
            {
                foreach (var memoryDb in memoryDbs)
                {
                    var list = memoryDb.GetListAsync(memoryIndex.Name, null, 100, true);

                    await foreach (var item in list)
                    {
                        if (item.Id.Contains(fileid))
                        {
                            var test = item.Payload.FirstOrDefault(p => p.Key == "text");
                            docTextList.Add(test.Value.ConvertToString());
                        }
                    }
                }
            }
            return docTextList;
        }
        private async Task FileUpload()
        {
           
        }

        public class UrlModel
        {
            [Required]
            public string Url { get; set; }
        }
        private async Task UrlHandleOk(MouseEventArgs e)
        {
            try
            {
                _urlConfirmLoading = true;
                string fileid = Guid.NewGuid().ToString();
                await _memory.ImportWebPageAsync(urlModel.Url, fileid, new TagCollection() { { "kmsid", KmsId } }
                     , index: "kms");
                //查询文档数量
                List<string> docTextList =await GetDocumentByFileID(fileid);

                KmsDetails detial = new KmsDetails()
                {
                    Id = fileid,
                    KmsId = KmsId,
                    Type = "url",
                    Url = urlModel.Url,
                    DataCount= docTextList.Count,
                    CreateTime=DateTime.Now
                };
                await _kmsDetails_Repositories.InsertAsync(detial);
                _data = await _kmsDetails_Repositories.GetListAsync(p => p.KmsId == KmsId);

                _urlVisible = false;
                _urlConfirmLoading = false;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message+" ---- "+ex.StackTrace);
            }
        }
        private void UrlHandleCancel(MouseEventArgs e)
        {
            _urlVisible = false;
        }
        private void ShowUrlModal()
        {
            _urlVisible = true;
        }
    }
}

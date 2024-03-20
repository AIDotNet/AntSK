using AntDesign;
using AntSK.BackgroundTask;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Domain.Model;
using AntSK.Domain.Repositories;
using AntSK.Models;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.KernelMemory;
using System.ComponentModel.DataAnnotations;
using static AntSK.Pages.KmsPage.KmsDetail;

namespace AntSK.Pages.KmsPage
{
    public partial class KmsDetail
    {
        [Parameter]
        public string KmsId { get; set; }

        private readonly KmsDetails _model = new KmsDetails();

        private bool _urlVisible = false;
        private bool _urlConfirmLoading = false;

        private bool _fileVisible = false;
        private bool _fileConfirmLoading = false;

        private bool _textVisible = false;
        private bool _textConfirmLoading = false;

        private List<FileInfoModel> fileList = new List<FileInfoModel>();

        private Form<UrlModel> _urlForm;
        private UrlModel urlModel = new UrlModel();

        private Form<TextModel> _textForm;
        private TextModel textModel = new TextModel();

        private readonly IDictionary<string, ProgressStatus> _pStatus = new Dictionary<string, ProgressStatus>
        {
            {"active", ProgressStatus.Active},
            {"exception", ProgressStatus.Exception},
            {"normal", ProgressStatus.Normal},
            {"success", ProgressStatus.Success}
        };

        private List<KmsDetails> _data = new List<KmsDetails>();

        [Inject]
        protected IConfirmService _confirmService { get; set; }

        [Inject]
        protected IKmsDetails_Repositories _kmsDetails_Repositories { get; set; }

        [Inject]
        protected IKmss_Repositories _kmss_Repositories { get; set; }

        private MemoryServerless _memory { get; set; }

        [Inject]
        protected IKMService iKMService { get; set; }

        [Inject]
        protected MessageService? _message { get; set; }

        [Inject]
        protected BackgroundTaskBroker<ImportKMSTaskReq> _taskBroker { get; set; }

        [Inject]
        protected IHttpService _httpService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _data = await _kmsDetails_Repositories.GetListAsync(p => p.KmsId == KmsId);
            var km = _kmss_Repositories.GetFirst(p => p.Id == KmsId);
            //使用知识库设置的参数，
            _memory = iKMService.GetMemoryByKMS(km.Id);
        }

        //刷新
        private async Task Refresh()
        {
            _data = await _kmsDetails_Repositories.GetListAsync(p => p.KmsId == KmsId);
        }

        /// <summary>
        /// 根据文档ID获取文档
        /// </summary>
        /// <param name="fileid"></param>
        /// <returns></returns>

        #region Url

        public class UrlModel
        {
            [Required]
            public string Url { get; set; }
        }

        private async Task UrlHandleOk(MouseEventArgs e)
        {
            try
            {
                var result = await _httpService.PostAsync(NavigationManager.BaseUri + "api/KMS/ImportKMSTask", new ImportKMSTaskDTO()
                {
                    ImportType = ImportType.Url,
                    KmsId = KmsId,
                    Url = urlModel.Url,
                });
                _data = await _kmsDetails_Repositories.GetListAsync(p => p.KmsId == KmsId);
                _urlVisible = false;
                urlModel.Url = "";
                _ = _message.Info("加入队列，进入后台处理中！", 2);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message + " ---- " + ex.StackTrace);
            }
        }

        private void UrlHandleCancel(MouseEventArgs e)
        {
            _urlVisible = false;
        }

        private void UrlShowModal()
        {
            _urlVisible = true;
        }

        #endregion Url

        #region Text

        public class TextModel
        {
            [Required]
            public string Text { get; set; }
        }

        private async Task TextHandleOk(MouseEventArgs e)
        {
            try
            {
                var result = await _httpService.PostAsync(NavigationManager.BaseUri + "api/KMS/ImportKMSTask", new ImportKMSTaskDTO()
                {
                    ImportType = ImportType.Text,
                    KmsId = KmsId,
                    Text = textModel.Text
                });
                _data = await _kmsDetails_Repositories.GetListAsync(p => p.KmsId == KmsId);
                _textVisible = false;
                textModel.Text = "";
                _ = _message.Info("加入队列，进入后台处理中！", 2);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message + " ---- " + ex.StackTrace);
            }
        }

        private void TextHandleCancel(MouseEventArgs e)
        {
            _textVisible = false;
        }

        private void TextShowModal()
        {
            _textVisible = true;
        }

        #endregion Text

        #region File

        private async Task FileHandleOk(MouseEventArgs e)
        {
            try
            {
                foreach (var item in fileList)
                {
                    var result = await _httpService.PostAsync(NavigationManager.BaseUri + "api/KMS/ImportKMSTask", new ImportKMSTaskDTO()
                    {
                        ImportType = ImportType.File,
                        KmsId = KmsId,
                        FilePath = item.FilePath,
                        FileName = item.FileName
                    });
                }
                _data = await _kmsDetails_Repositories.GetListAsync(p => p.KmsId == KmsId);
                //上传文档
                _fileVisible = false;
                fileList.Clear();
                _ = _message.Info("加入队列，进入后台处理中！", 2);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message + " ---- " + ex.StackTrace);
            }
        }

        private void FileHandleCancel(MouseEventArgs e)
        {
            _fileVisible = false;
        }

        private void FileShowModal()
        {
            _fileVisible = true;
        }

        private void OnSingleCompleted(UploadInfo fileinfo)
        {
            if (fileinfo.File.State == UploadState.Success)
            {
                //文件列表
                fileList.Add(new FileInfoModel()
                {
                    FileName = fileinfo.File.FileName,
                    FilePath = fileinfo.File.Url = fileinfo.File.Response
                });
            }
        }

        private void FileDetail(string fileid)
        {
            NavigationManager.NavigateTo($"/kms/detaillist/{KmsId}/{fileid}");
        }

        private async Task DeleteFile(string fileid)
        {
            try
            {
                var content = "是否确认删除此文档?";
                var title = "删除";
                var result = await _confirmService.Show(content, title, ConfirmButtons.YesNo);
                if (result == ConfirmResult.Yes)
                {
                    var flag = await _kmsDetails_Repositories.DeleteAsync(fileid);
                    if (flag)
                    {
                        await _memory.DeleteDocumentAsync(index: "kms", documentId: fileid);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message + " ---- " + ex.StackTrace);
            }
            finally
            {
                _data = await _kmsDetails_Repositories.GetListAsync(p => p.KmsId == KmsId);
                await InvokeAsync(StateHasChanged);
            }
        }

        #endregion File
    }
}
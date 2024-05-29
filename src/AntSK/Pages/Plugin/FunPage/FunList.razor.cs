using AntDesign;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Domain.Model.Fun;
using AntSK.Domain.Domain.Service;
using AntSK.Domain.Repositories;
using AntSK.Models;
using AntSK.Pages.KmsPage;
using DocumentFormat.OpenXml.Office2010.Excel;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.SemanticKernel;

namespace AntSK.Pages.FunPage
{
    public partial class FunList
    {
        private FunDto[] _data = { };

        [Inject]
        FunctionService _functionService { get; set; }
        [Inject]
        IServiceProvider _serviceProvider { get; set; }
        [Inject]
        IConfirmService _confirmService { get; set; }
        [Inject]
        IFuns_Repositories _funs_Repositories { get; set; }

        [Inject]
        protected MessageService? _message { get; set; }
        [Inject] protected ILogger<FunDto> _logger { get; set; }

        bool _fileVisible = false;
        bool _fileConfirmLoading = false;
        List<FileInfoModel> fileList = new List<FileInfoModel>();


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await InitData("");
        }

        private async Task InitData(string searchKey)
        {
            var list = new List<FunDto> { new FunDto() };

            _functionService.SearchMarkedMethods();
            using var scope = _serviceProvider.CreateScope();

            var funList = _functionService.Functions;
            if (!string.IsNullOrEmpty(searchKey))
            {
                funList = funList.Where(x => x.Key.Contains(searchKey)).ToDictionary(x => x.Key, x => x.Value);
            }
            foreach (var func in funList)
            {
                var methodInfo = _functionService.MethodInfos[func.Key];
                list.Add(new FunDto() { Name = func.Key, Description = methodInfo.Description });
            }
            _data = list.ToArray();
            await InvokeAsync(StateHasChanged);
        }

        private void NavigateToAddApp()
        {
            NavigationManager.NavigateTo("/plugins/fun/add");
        }

        private async Task Search(string searchKey)
        {
            await InitData(searchKey);
        }

        private async Task AddFun() {
            _fileVisible = true;
        }
        private async Task ClearFun()
        {
            var content = "清空自定义函数将会删除全部导入函数，并且需要程序重启后下次生效，如不是DLL冲突等原因不建议清空，是否要清空？";
            var title = "清空自定义函数";
            var result = await _confirmService.Show(content, title, ConfirmButtons.YesNo);
            if (result == ConfirmResult.Yes)
            {
                await _funs_Repositories.DeleteAsync(p=>true);
                await InitData("");
            }
        }


        private async Task FileHandleOk(MouseEventArgs e)
        {
            try
            {
                foreach (var file in fileList)
                {
                    _funs_Repositories.Insert(new Funs() { Id = Guid.NewGuid().ToString(), Path = file.FilePath });
                    _functionService.FuncLoad(file.FilePath);
                }
                _message.Info("上传成功");
                await InitData("");
                _fileVisible = false;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message + " ---- " + ex.StackTrace);
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

        bool BeforeUpload(UploadFileItem file)
        {
            if (file.Ext != ".dll")
            {
                _message.Error("请上传dll文件!");
            }
            var IsLt500K = file.Size < 1024 * 1024 * 100;
            if (!IsLt500K)
            {
                _message.Error("文件需不大于100MB!");
            }

            return  IsLt500K;
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
    }
}

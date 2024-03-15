using AntDesign;
using AntSK.Domain.Domain.Model.Fun;
using AntSK.Domain.Domain.Service;
using AntSK.Domain.Repositories;
using AntSK.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.SemanticKernel;

namespace AntSK.Pages.ApiPage
{
    public partial class FunList
    {
        private Funs[] _data = { };

        [Inject]
        FunctionService _functionService { get; set; }
        [Inject]
        IServiceProvider _serviceProvider { get; set; }
        [Inject]
        IConfirmService _confirmService { get; set; }

        [Inject]
        protected MessageService? _message { get; set; }

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
            var list = new List<Funs> { new Funs() };

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
                list.Add(new Funs() { Name = func.Key, Description = methodInfo.Description });
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


        private async Task FileHandleOk(MouseEventArgs e)
        {
            try
            {
                
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

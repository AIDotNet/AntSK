using AntDesign;
using Microsoft.AspNetCore.Components;
using AntSK.Domain.Repositories;
using AntSK.Models;
using System.IO;

namespace AntSK.Pages.ApiPage
{
    public partial class AddApi
    {
        [Parameter]
        public string ApiId { get; set; }

        [Inject]
        protected IApis_Repositories _apis_Repositories { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        protected MessageService? Message { get; set; }

        private Apis _apiModel = new Apis() ;


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (!string.IsNullOrEmpty(ApiId))
            {
                //查看
                _apiModel = _apis_Repositories.GetFirst(p => p.Id == ApiId);
            }
        }
        private void HandleSubmit()
        {
            if (string.IsNullOrEmpty(ApiId))
            {
                //新增
                _apiModel.Id = Guid.NewGuid().ToString();
               
                if (_apis_Repositories.IsAny(p => p.Name == _apiModel.Name))
                {
                    _ = Message.Error("名称已存在！", 2);
                    return;
                }
                _apis_Repositories.Insert(_apiModel);
            }
            else {
                //修改

                _apis_Repositories.Update(_apiModel);
            }

            Back();
        }


        private void Back() {
            NavigationManager.NavigateTo("/plugins/apilist");
        }
    }

}

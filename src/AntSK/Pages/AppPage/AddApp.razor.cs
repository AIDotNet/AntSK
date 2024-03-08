using AntDesign;
using Microsoft.AspNetCore.Components;
using AntSK.Domain.Repositories;
using AntSK.Models;
using System.IO;
using AntSK.Domain.Model.Enum;

namespace AntSK.Pages.AppPage
{
    public partial class AddApp
    {
        [Parameter]
        public string AppId { get; set; }

        [Inject]
        protected IApps_Repositories _apps_Repositories { get; set; }

        [Inject]
        protected IKmss_Repositories _kmss_Repositories { get; set; }

        [Inject]
        protected IApis_Repositories _apis_Repositories { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        protected MessageService? Message { get; set; }
        [Inject]
        protected IAIModels_Repositories _aimodels_Repositories { get; set; }

        private Apps _appModel = new Apps() ;

        IEnumerable <string>  kmsIds;

        private List<Kmss> _kmsList = new List<Kmss>();

        IEnumerable<string> apiIds;

        private List<Apis> _apiList = new List<Apis>();

        private List<AIModels> _chatList { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _kmsList = _kmss_Repositories.GetList();
            _apiList= _apis_Repositories.GetList();

            _chatList= _aimodels_Repositories.GetList(p => p.AIModelType == AIModelType.Chat);

            if (!string.IsNullOrEmpty(AppId))
            {
                //查看
                _appModel= _apps_Repositories.GetFirst(p => p.Id == AppId);
                kmsIds = _appModel.KmsIdList?.Split(",");
                apiIds= _appModel.ApiFunctionList?.Split(",");
            }


        }
        private void HandleSubmit()
        {
            if (string.IsNullOrEmpty(AppId))
            {
                //新增
                _appModel.Id = Guid.NewGuid().ToString();
                //秘钥
                _appModel.SecretKey="sk-"+ Guid.NewGuid().ToString();
                if (_apps_Repositories.IsAny(p => p.Name == _appModel.Name))
                {
                    _ = Message.Error("名称已存在！", 2);
                    return;
                }

                if (kmsIds != null && kmsIds.Count() > 0)
                {
                    _appModel.KmsIdList = string.Join(",", kmsIds);
                }
                if (apiIds != null && apiIds.Count() > 0)
                {
                    _appModel.ApiFunctionList = string.Join(",", apiIds);
                }
                _apps_Repositories.Insert(_appModel);
            }
            else {
                //修改
                if (kmsIds != null && kmsIds.Count() > 0)
                {
                    _appModel.KmsIdList = string.Join(",", kmsIds);
                }
                if (apiIds != null && apiIds.Count() > 0)
                {
                    _appModel.ApiFunctionList = string.Join(",", apiIds);
                }
                _apps_Repositories.Update(_appModel);
            }
          

            //NavigationManager.NavigateTo($"/app/detail/{_appModel.Id}");
            NavigationManager.NavigateTo($"/applist");
        }


        private void Back() {
            NavigationManager.NavigateTo("/applist");
        }
    }

}

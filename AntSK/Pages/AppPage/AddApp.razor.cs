using AntDesign;
using Microsoft.AspNetCore.Components;
using AntSK.Domain.Repositories;
using AntSK.Models;
using System.IO;

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
        protected NavigationManager NavigationManager { get; set; }

        private Apps _appModel = new Apps() ;

        IEnumerable <string>  kmsIds;

        private List<Kmss> _kmsList = new List<Kmss>();


        private string _errorMsg { get; set; }

        private readonly FormItemLayout _formItemLayout = new FormItemLayout
        {
            LabelCol = new ColLayoutParam
            {
                Xs = new EmbeddedProperty { Span = 24 },
                Sm = new EmbeddedProperty { Span = 7 },
            },

            WrapperCol = new ColLayoutParam
            {
                Xs = new EmbeddedProperty { Span = 24 },
                Sm = new EmbeddedProperty { Span = 12 },
                Md = new EmbeddedProperty { Span = 10 },
            }
        };
        private readonly FormItemLayout _submitFormLayout = new FormItemLayout
        {
            WrapperCol = new ColLayoutParam
            {
                Xs = new EmbeddedProperty { Span = 24, Offset = 0 },
                Sm = new EmbeddedProperty { Span = 10, Offset = 7 },
            }
        };
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _kmsList = _kmss_Repositories.GetList();
            if (!string.IsNullOrEmpty(AppId))
            {
                //查看
                _appModel= _apps_Repositories.GetFirst(p => p.Id == AppId);
                kmsIds = _appModel.KmsIdList?.Split(",");
            }
        }
        private void HandleSubmit()
        {
            if (string.IsNullOrEmpty(AppId))
            {
                //新增
                _appModel.Id = Guid.NewGuid().ToString();
                if (_apps_Repositories.IsAny(p => p.Name == _appModel.Name))
                {
                    _errorMsg = "名称已存在！";
                    return;
                }

                if (kmsIds != null && kmsIds.Count() > 0)
                {
                    _appModel.KmsIdList = string.Join(",", kmsIds);
                }

                _apps_Repositories.Insert(_appModel);
            }
            else {
                //修改
                if (kmsIds != null && kmsIds.Count() > 0)
                {
                    _appModel.KmsIdList = string.Join(",", kmsIds);
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

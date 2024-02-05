using AntDesign;
using Microsoft.AspNetCore.Components;
using AntSK.Domain.Repositories;
using AntSK.Models;
using System.IO;

namespace AntSK.Pages.AppPage
{
    public partial class AddApp
    {
        [Inject]
        protected IApps_Repositories apps_Repositories { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        private readonly Apps _appModel = new Apps() ;
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

        private void HandleSubmit()
        {
            _appModel.Id = Guid.NewGuid().ToString();
            if (apps_Repositories.IsAny(p => p.Name == _appModel.Name))
            {
                _errorMsg = "名称已存在！";
                return;
            }

            apps_Repositories.Insert(_appModel);

            NavigationManager.NavigateTo($"/app/detail/{_appModel.Id}");

        }
    }
    
}

using AntDesign;
using Microsoft.AspNetCore.Components;
using AntSK.Domain.Repositories;
using AntSK.Models;
using System.IO;

namespace AntSK.Pages.Kms
{

    public partial class AddKms
    {
        [Inject]
        protected IKmss_Repositories _kmss_Repositories { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        private string _errorMsg { get; set; }

        private readonly Kmss _kmsModel = new Kmss() {  ChatModel="gpt4-turbo",EmbeddingModel= "text-embedding-ada-002" };

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
            _kmsModel.Id = Guid.NewGuid().ToString();
            if (_kmss_Repositories.IsAny(p => p.Name == _kmsModel.Name))
            {
                _errorMsg = "名称已存在！";
                return;
            }

            _kmss_Repositories.Insert(_kmsModel);

            NavigationManager.NavigateTo("/kmslist");

        }
    }    
}

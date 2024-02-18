using AntDesign;
using AntSK.Domain.Model;
using AntSK.Domain.Repositories;
using AntSK.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace AntSK.Pages.AppPage
{
    public partial class AppOpen
    {
        [Parameter]
        public string AppId { get; set; }

        [Inject]
        protected IApps_Repositories _apps_Repositories { get; set; }
        [Inject]
        IConfirmService _confirmService { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

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

        private Apps _appModel = new Apps();

        private string _openApiUrl  { get; set; }

        private string _desc { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _appModel = _apps_Repositories.GetFirst(p => p.Id == AppId);
            _openApiUrl = NavigationManager.BaseUri + "api/v1/chat/completions";
            GetDesc();
        }

        private void GetDesc()
        {
            _desc = @$"为了方便其他应用对接，接口符合openai规范，省略了温度TopP等参数。{Environment.NewLine}BaseUrl:{Environment.NewLine}{_openApiUrl} {Environment.NewLine}headers:{Environment.NewLine}Authorization: ""{_appModel.SecretKey}"" {Environment.NewLine}Body:  {Environment.NewLine}{JsonConvert.SerializeObject(new OpenAIModel() { messages=new List<OpenAIMessage>() { new OpenAIMessage() {  role="user",content="你好，你是谁"} } }, Formatting.Indented)}";
        }

        private void HandleSubmit()
        {
        }

        private async Task Reset() 
        {
            var content = "是否确认重置秘钥，重之后之前的秘钥将无法访问API接口";
            var title = "重置";
            var result = await _confirmService.Show(content, title, ConfirmButtons.YesNo);
            if (result == ConfirmResult.Yes)
            {
                _appModel.SecretKey = "sk-" + Guid.NewGuid().ToString();
                _apps_Repositories.Update(_appModel);
                GetDesc();
            }
           
        }
    }
}

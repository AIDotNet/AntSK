using AntDesign;
using AntDesign.ProLayout;
using AntSK.Domain.Options;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components;

namespace AntSK.Pages.Setting.AIModel
{
    public partial class AddModel
    {
        [Parameter]
        public string ModelId { get; set; }
        [Inject] protected IAIModels_Repositories _aimodels_Repositories { get; set; }
        [Inject] protected MessageService? Message { get; set; }
        [Inject] public HttpClient HttpClient { get; set; }

        private AIModels _aiModel = new AIModels();

        IEnumerable<string> _menuKeys;

        private List<MenuDataItem> menuList = new List<MenuDataItem>();
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (!string.IsNullOrEmpty(ModelId))
            {
                _aiModel= _aimodels_Repositories.GetFirst(p => p.Id == ModelId);
            }
        }

        private void HandleSubmit()
        {
            if (string.IsNullOrEmpty(ModelId))
            {
                //新增
                _aiModel.Id = Guid.NewGuid().ToString();

                if (_aimodels_Repositories.IsAny(p => p.ModelDescription == _aiModel.ModelDescription ))
                {
                    _ = Message.Error("模型描述已存在！", 2);
                    return;
                }

                if (_aimodels_Repositories.IsAny(p =>p.AIModelType==_aiModel.AIModelType&& p.EndPoint == _aiModel.EndPoint&&p.ModelKey==_aiModel.ModelKey&&p.ModelName==_aiModel.ModelName))
                {
                    _ = Message.Error("模型已存在！", 2);
                    return;
                }

                _aimodels_Repositories.Insert(_aiModel);
            }
            else
            {
                _aimodels_Repositories.Update(_aiModel);
            }

            Back();
        }

        private void Back() 
        {
            NavigationManager.NavigateTo("/setting/modellist");
        }
    }
}

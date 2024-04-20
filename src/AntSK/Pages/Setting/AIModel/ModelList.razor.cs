using AntDesign;
using AntSK.Domain.Repositories;
using AntSK.Models;
using Microsoft.AspNetCore.Components;

namespace AntSK.Pages.Setting.AIModel
{
    public partial class ModelList
    {
        private readonly BasicListFormModel _model = new BasicListFormModel();

        private List<AIModels> _data;

        private string _searchKeyword;

        [Inject]
        protected IAIModels_Repositories _aIModels_Repositories { get; set; }

        [Inject]
        protected IApps_Repositories _apps_Repositories{ get; set; }
        [Inject]
        protected IKmss_Repositories _kmss_Repositories { get; set; }

        [Inject] protected MessageService? Message { get; set; }

        [Inject]
        IConfirmService _confirmService { get; set; }


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await InitData();
        }
        private async Task InitData(string searchKey = null)
        {
            if (string.IsNullOrEmpty(searchKey))
            {
                _data = _aIModels_Repositories.GetList();
            }
            else
            {
                _data = _aIModels_Repositories.GetList(p => p.ModelName.Contains(searchKey) || p.ModelDescription.Contains(searchKey));
            }
            await InvokeAsync(StateHasChanged);
        }
        public async Task OnSearch()
        {
            await InitData(_searchKeyword);
        }

        public async Task AddModel()
        {
            NavigationManager.NavigateTo("/modelmanager/model/add");
        }

        public void Edit(string modelid)
        {
            NavigationManager.NavigateTo("/modelmanager/model/add/" + modelid);
        }

        public async Task Delete(string modelid)
        {
            var content = "是否确认删除此模型";
            var title = "删除";
            var result = await _confirmService.Show(content, title, ConfirmButtons.YesNo);
            if (result == ConfirmResult.Yes)
            {
                if (_apps_Repositories.IsAny(p => p.ChatModelID == modelid || p.EmbeddingModelID == modelid))
                {
                    _ = Message.Error("该模型有应用在使用，请先删除应用后才允许删除该模型");
                    return;
                }
                if (_kmss_Repositories.IsAny(p => p.ChatModelID == modelid || p.EmbeddingModelID == modelid))
                {
                    _ = Message.Error("该模型有知识库在使用，请先删除知识库后才允许删除该模型");
                    return;
                }
                await _aIModels_Repositories.DeleteAsync(modelid);
                await InitData("");
            }
        }
    }
}

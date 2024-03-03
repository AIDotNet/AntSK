using AntDesign;
using AntSK.Domain.Repositories;
using AntSK.Models;
using AntSK.Services;
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


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await InitData();
        }
        private async Task InitData(string searchKey=null)
        {
            if (string.IsNullOrEmpty(searchKey))
            {
                _data = _aIModels_Repositories.GetList();
            }
            else
            {
                _data = _aIModels_Repositories.GetList(p=>p.ModelName.Contains(searchKey)||p.ModelDescription.Contains(searchKey));
            }
        }
        public async Task OnSearch() {
            await InitData(_searchKeyword);
        }

        public async Task AddModel() {
            NavigationManager.NavigateTo("/setting/model/add");
        }

        public void Edit(string modelid)
        {
            NavigationManager.NavigateTo("/setting/model/add/"+ modelid);
        }
    }
}

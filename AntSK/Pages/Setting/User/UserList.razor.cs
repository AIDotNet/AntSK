using AntDesign;
using AntSK.Domain.Repositories;
using AntSK.Models;
using AntSK.Services;
using Microsoft.AspNetCore.Components;

namespace AntSK.Pages.Setting.User
{
    public partial class UserList
    {
        private readonly BasicListFormModel _model = new BasicListFormModel();

        private readonly IDictionary<string, ProgressStatus> _pStatus = new Dictionary<string, ProgressStatus>
        {
            {"active", ProgressStatus.Active},
            {"exception", ProgressStatus.Exception},
            {"normal", ProgressStatus.Normal},
            {"success", ProgressStatus.Success}
        };

        private List<Users> _data;

        private string _searchKeyword;

        [Inject] 
        protected IUsers_Repositories _users_Repositories { get; set; }


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await InitData();
        }
        private async Task InitData(string searchKey=null)
        {
            if (string.IsNullOrEmpty(searchKey))
            {
                _data = _users_Repositories.GetList();
            }
            else
            {
                _data = _users_Repositories.GetList(p=>p.Name.Contains(searchKey)||p.Describe.Contains(searchKey)||p.No.Contains(searchKey));
            }
        }
        public async Task OnSearch() {
            await InitData(_searchKeyword);
        }

        public async Task AddUser() {
            NavigationManager.NavigateTo("/setting/user/add");
        }

        public void Edit(string userid)
        {
            NavigationManager.NavigateTo("/setting/user/add/"+userid);
        }
    }
}

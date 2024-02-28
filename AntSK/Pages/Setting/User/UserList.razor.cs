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
            _data = _users_Repositories.GetList();
        }

        public void AddUser() {
            NavigationManager.NavigateTo("/setting/user/add");
        }
    }
}

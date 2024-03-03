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

        private List<Users> _data;

        private string _searchKeyword;

        [Inject] 
        protected IUsers_Repositories _users_Repositories { get; set; }
        [Inject]
        IConfirmService _confirmService { get; set; }

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
            await InvokeAsync(StateHasChanged);
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

        public async Task Delete(string modelid)
        {
            var content = "是否确认删除此用户";
            var title = "删除";
            var result = await _confirmService.Show(content, title, ConfirmButtons.YesNo);
            if (result == ConfirmResult.Yes)
            {
                await _users_Repositories.DeleteAsync(modelid);
                await InitData("");
            }
        }
    }
}

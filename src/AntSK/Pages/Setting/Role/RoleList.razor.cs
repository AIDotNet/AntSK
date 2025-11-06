using AntDesign;
using AntSK.Domain.Repositories;
using AntSK.Models;
using Microsoft.AspNetCore.Components;

namespace AntSK.Pages.Setting.Role
{
    public partial class RoleList
    {
        private readonly BasicListFormModel _model = new BasicListFormModel();

        private List<Roles> _data;

        private string _searchKeyword;

        [Inject]
        protected IRoles_Repositories _roles_Repositories { get; set; }
        [Inject]
        protected IRolePermissions_Repositories _rolePermissions_Repositories { get; set; }
        [Inject]
        protected IUserRoles_Repositories _userRoles_Repositories { get; set; }
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
                _data = _roles_Repositories.GetList();
            }
            else
            {
                _data = _roles_Repositories.GetList(p => p.Name.Contains(searchKey) || p.Code.Contains(searchKey) || (p.Description != null && p.Description.Contains(searchKey)));
            }
            await InvokeAsync(StateHasChanged);
        }
        public async Task OnSearch()
        {
            await InitData(_searchKeyword);
        }

        public async Task AddRole()
        {
            NavigationManager.NavigateTo("/setting/role/add");
        }

        public void Edit(string roleid)
        {
            NavigationManager.NavigateTo("/setting/role/add/" + roleid);
        }

        public async Task Delete(string roleid)
        {
            var content = "是否确认删除此角色";
            var title = "删除";
            var result = await _confirmService.Show(content, title, ConfirmButtons.YesNo);
            if (result == ConfirmResult.Yes)
            {
                // 删除角色权限关联
                var rolePerms = _rolePermissions_Repositories.GetList(p => p.RoleId == roleid);
                foreach (var rp in rolePerms)
                {
                    await _rolePermissions_Repositories.DeleteAsync(rp.Id);
                }

                // 删除用户角色关联
                var userRoles = _userRoles_Repositories.GetList(p => p.RoleId == roleid);
                foreach (var ur in userRoles)
                {
                    await _userRoles_Repositories.DeleteAsync(ur.Id);
                }

                // 删除角色
                await _roles_Repositories.DeleteAsync(roleid);
                await InitData("");
            }
        }
    }
}

using AntDesign;
using AntDesign.ProLayout;
using AntSK.Domain.Options;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using Microsoft.AspNetCore.Components;

namespace AntSK.Pages.Setting.User
{
    public partial class AddUser
    {
        [Parameter]
        public string UserId { get; set; }
        [Inject] protected IUsers_Repositories _users_Repositories { get; set; }
        [Inject] protected IRoles_Repositories _roles_Repositories { get; set; }
        [Inject] protected IUserRoles_Repositories _userRoles_Repositories { get; set; }
        [Inject] protected MessageService? Message { get; set; }
        [Inject] public HttpClient HttpClient { get; set; }

        private Users _userModel = new Users();
        private string _password = "";
        IEnumerable<string> _menuKeys;
        IEnumerable<string> _roleIds;

        private List<MenuDataItem> menuList = new List<MenuDataItem>();
        private List<Roles> _allRoles = new List<Roles>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            
            // 加载所有角色
            _allRoles = _roles_Repositories.GetList(r => r.IsEnabled);

            if (!string.IsNullOrEmpty(UserId))
            {
                _userModel = _users_Repositories.GetFirst(p => p.Id == UserId);
                _password = _userModel.Password;

                // 加载用户已有的角色
                var userRoles = _userRoles_Repositories.GetList(p => p.UserId == UserId);
                _roleIds = userRoles.Select(ur => ur.RoleId);
            }
            menuList = (await HttpClient.GetFromJsonAsync<MenuDataItem[]>("data/menu.json")).ToList().Where(p => p.Key != "setting").ToList();
            _menuKeys = _userModel.MenuRole?.Split(",");
        }

        private void HandleSubmit()
        {
            _userModel.MenuRole = string.Join(",", _menuKeys);
            if (string.IsNullOrEmpty(UserId))
            {
                //新增
                _userModel.Id = Guid.NewGuid().ToString();

                if (_userModel.No == LoginOption.User)
                {
                    _ = Message.Error("工号不能为管理员账号！", 2);
                    return;
                }
                if (_users_Repositories.IsAny(p => p.No == _userModel.No))
                {
                    _ = Message.Error("工号已存在！", 2);
                    return;
                }
                _userModel.Password = PasswordUtil.HashPassword(_userModel.Password);
                _users_Repositories.Insert(_userModel);

                // 添加用户角色关联
                if (_roleIds != null)
                {
                    foreach (var roleId in _roleIds)
                    {
                        _userRoles_Repositories.Insert(new UserRoles
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = _userModel.Id,
                            RoleId = roleId,
                            CreateTime = DateTime.Now
                        });
                    }
                }
            }
            else
            {
                //修改
                if (_userModel.Password != _password)
                {
                    _userModel.Password = PasswordUtil.HashPassword(_userModel.Password);
                }
                _users_Repositories.Update(_userModel);

                // 先删除旧的用户角色关联
                var oldUserRoles = _userRoles_Repositories.GetList(p => p.UserId == UserId);
                foreach (var ur in oldUserRoles)
                {
                    _userRoles_Repositories.Delete(ur.Id);
                }

                // 添加新的用户角色关联
                if (_roleIds != null)
                {
                    foreach (var roleId in _roleIds)
                    {
                        _userRoles_Repositories.Insert(new UserRoles
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = _userModel.Id,
                            RoleId = roleId,
                            CreateTime = DateTime.Now
                        });
                    }
                }
            }

            Back();
        }

        private void Back()
        {
            NavigationManager.NavigateTo("/setting/userlist");
        }
    }
}

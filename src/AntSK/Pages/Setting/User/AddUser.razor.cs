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
        [Inject] protected MessageService? Message { get; set; }
        [Inject] public HttpClient HttpClient { get; set; }

        private Users _userModel = new Users();
        private string _password = "";
        IEnumerable<string> _menuKeys;

        private List<MenuDataItem> menuList = new List<MenuDataItem>();
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (!string.IsNullOrEmpty(UserId))
            {
                _userModel = _users_Repositories.GetFirst(p => p.Id == UserId);
                _password = _userModel.Password;
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
            }
            else
            {
                //修改
                if (_userModel.Password != _password)
                {
                    _userModel.Password = PasswordUtil.HashPassword(_userModel.Password);
                }
                _users_Repositories.Update(_userModel);
            }

            Back();
        }

        private void Back()
        {
            NavigationManager.NavigateTo("/setting/userlist");
        }
    }
}

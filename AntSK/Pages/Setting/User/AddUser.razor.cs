using AntDesign;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components;

namespace AntSK.Pages.Setting.User
{
    public partial class AddUser
    {
        [Parameter]
        public string UserId { get; set; }
        [Inject]
        protected IUsers_Repositories _users_Repositories { get; set; }
        [Inject]
        protected MessageService? Message { get; set; }

        private Users _userModel = new Users();
        private string _password = "";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

        }

        private void HandleSubmit()
        {
            if (string.IsNullOrEmpty(UserId))
            {
                //新增
                _userModel.Id = Guid.NewGuid().ToString();
                if (_users_Repositories.IsAny(p => p.No == _userModel.No))
                {
                    _ = Message.Info("工号已存在！", 2);
                    return;
                }

                _userModel.Password=PasswordUtil.HashPassword(_userModel.Password);
                _users_Repositories.Insert(_userModel);
            }
            else
            {
                //修改
                _userModel.Password = PasswordUtil.HashPassword(_userModel.Password);
                _users_Repositories.Update(_userModel);
            }

            NavigationManager.NavigateTo("/setting/userlist");
        }

        private void Back() 
        {
            NavigationManager.NavigateTo("/setting/userlist");
        }
    }
}

using AntDesign;
using AntSK.Domain.Options;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using DocumentFormat.OpenXml.InkML;
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
            if (!string.IsNullOrEmpty(UserId))
            {
                _userModel= _users_Repositories.GetFirst(p => p.Id == UserId);
                _password= _userModel.Password;
            }
        }

        private void HandleSubmit()
        {
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

                _userModel.Password=PasswordUtil.HashPassword(_userModel.Password);
                _users_Repositories.Insert(_userModel);
            }
            else
            {
                //修改
                if (_userModel.Password!=_password)
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

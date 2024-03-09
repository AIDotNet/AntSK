using AntDesign;
using AntSK.Models;
using AntSK.Services.Auth;
using Microsoft.AspNetCore.Components;

namespace AntSK.Pages.User
{
    public partial class Login
    {
        private readonly LoginParamsType _model = new LoginParamsType();

        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject] public MessageService Message { get; set; }

        public async Task HandleSubmit()
        {
            //判断是否管理员
            var loginFailed = await ((AntSKAuthProvider)AuthenticationStateProvider).SignIn(_model.UserName, _model.Password);
            if (loginFailed)
            {
                NavigationManager.NavigateTo("/");
                return;
            }
            else
            {
                Message.Error("账号密码错误", 2);
            }
        }
    }
}
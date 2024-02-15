using AntDesign;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using AntSK.Models;
using AntSK.Services;
using AntSK.Domain.Options;
using SqlSugar;
using AntSK.Services.Auth;

namespace AntSK.Pages.User
{
    public partial class Login
    {
        private readonly LoginParamsType _model = new LoginParamsType();

        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject] public IAccountService AccountService { get; set; }

        [Inject] public MessageService Message { get; set; }

        public async Task HandleSubmit()
        {
            var loginFailed = await((AntSKAuthProvider)AuthenticationStateProvider).SignIn(_model.UserName, _model.Password);
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
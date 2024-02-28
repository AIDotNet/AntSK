using AntSK.Domain.Repositories;
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

        private Users _userModel = new Users();
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

        }

        private void HandleSubmit()
        {

        }

        private void Back() 
        {
        
        }
    }
}

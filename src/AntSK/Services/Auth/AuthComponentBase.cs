using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace AntSK.Services.Auth
{
    public class AuthComponentBase : ComponentBase
    {
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public ClaimsPrincipal User { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await GetAuthenticationStateAsync();
        }

        private async Task GetAuthenticationStateAsync()
        {
            var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            User = authenticationState.User;

            if (!User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/user/login");
            }
        }
    }
}

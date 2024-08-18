using AntDesign;
using AntSK.Domain.Repositories;
using Microsoft.AspNetCore.Components;

namespace AntSK.Pages.AppPage
{
    public partial class AppList
    {
        private Apps[] _data = { };

        [Inject]
        protected IApps_Repositories _apps_Repositories { get; set; }
        [Inject]
        IConfirmService _confirmService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await InitData("");
        }

        private async Task InitData(string searchKey)
        {
            var list = new List<Apps> { new Apps() };
            List<Apps> data;
            if (string.IsNullOrEmpty(searchKey))
            {
                data = await _apps_Repositories.GetListAsync();
            }
            else
            {
                data = await _apps_Repositories.GetListAsync(p => p.Name.Contains(searchKey));
            }

            list.AddRange(data);
            _data = list.ToArray();
            await InvokeAsync(StateHasChanged);
        }

        private void NavigateToAddApp()
        {
            NavigationManager.NavigateTo("/app/add");
        }

        private async Task Search(string searchKey)
        {
            await InitData(searchKey);
        }

        private void Info(string id)
        {
            NavigationManager.NavigateTo($"/app/add/{id}");
        }

        private void Open(string id)
        {
            NavigationManager.NavigateTo($"/app/open/{id}");
        }

        private void Use(string id)
        {
            NavigationManager.NavigateTo($"/chat/{id}");
        }

        private async Task Delete(string id)
        {
            var content = "是否确认删除此应用";
            var title = "删除";
            var result = await _confirmService.Show(content, title, ConfirmButtons.YesNo);
            if (result == ConfirmResult.Yes)
            {
                await _apps_Repositories.DeleteAsync(id);
                await InitData("");
            }
        }
    }
}

using AntDesign;
using AntSK.Domain.Repositories;
using Microsoft.AspNetCore.Components;

namespace AntSK.Pages.ApiPage
{
    public partial class ApiList
    {
        private Apis[] _data = { };

        [Inject]
        protected IApis_Repositories _apis_Repositories { get; set; }
        [Inject]
        IConfirmService _confirmService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await InitData("");
        }

        private async Task InitData(string searchKey)
        {
            var list = new List<Apis> { new Apis() };
            List<Apis> data;
            if (string.IsNullOrEmpty(searchKey))
            {
                data = await _apis_Repositories.GetListAsync();
            }
            else
            {
                data = await _apis_Repositories.GetListAsync(p => p.Name.Contains(searchKey));
            }

            list.AddRange(data);
            _data = list.ToArray();
            await InvokeAsync(StateHasChanged);
        }

        private void NavigateToAddApp()
        {
            NavigationManager.NavigateTo("/plugins/api/add");
        }

        private async Task Search(string searchKey)
        {
            await InitData(searchKey);
        }

        private void Info(string id)
        {
            NavigationManager.NavigateTo($"/plugins/api/add/{id}");
        }

        private async Task Delete(string id)
        {
            var content = "是否确认删除此Api";
            var title = "删除";
            var result = await _confirmService.Show(content, title, ConfirmButtons.YesNo);
            if (result == ConfirmResult.Yes)
            {
                await _apis_Repositories.DeleteAsync(id);
                await InitData("");
            }
        }
    }
}

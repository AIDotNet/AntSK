using AntDesign;
using Microsoft.AspNetCore.Components;
using AntSK.Domain.Repositories;
using AntSK.Models;
using AntSK.Services;

namespace AntSK.Pages
{
    public partial class AppList
    {
        private readonly ListGridType _listGridType = new ListGridType
        {
            Gutter = 16,
            Xs = 1,
            Sm = 2,
            Md = 3,
            Lg = 3,
            Xl = 4,
            Xxl = 4
        };

        private Apps [] _data = { };

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

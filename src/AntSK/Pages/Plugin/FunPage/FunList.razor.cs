using AntDesign;
using AntSK.Domain.Domain.Model.Fun;
using AntSK.Domain.Domain.Service;
using AntSK.Domain.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;

namespace AntSK.Pages.ApiPage
{
    public partial class FunList
    {
        private Funs[] _data = { };

        [Inject]
        FunctionService _functionService { get; set; }
        [Inject]
        IServiceProvider _serviceProvider { get; set; }
        [Inject]
        IConfirmService _confirmService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await InitData("");
        }

        private async Task InitData(string searchKey)
        {
            var list = new List<Funs> { new Funs() };

            _functionService.SearchMarkedMethods();
            using var scope = _serviceProvider.CreateScope();

            var funList = _functionService.Functions;
            if (!string.IsNullOrEmpty(searchKey))
            {
                funList = funList.Where(x => x.Key.Contains(searchKey)).ToDictionary(x => x.Key, x => x.Value);
            }
            foreach (var func in funList)
            {

                var methodInfo = _functionService.MethodInfos[func.Key];
                list.Add(new Funs() { Name = func.Key, Description = methodInfo.Description });
            }
            _data = list.ToArray();
            await InvokeAsync(StateHasChanged);
        }

        private void NavigateToAddApp()
        {
            NavigationManager.NavigateTo("/plugins/fun/add");
        }

        private async Task Search(string searchKey)
        {
            await InitData(searchKey);
        }
    
    }
}

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

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var list = new List<Apps> { new Apps() };
            var data = await _apps_Repositories.GetListAsync();
            list.AddRange(data);
            _data = list.ToArray();
        }
    }
}

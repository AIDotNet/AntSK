using AntDesign;
using Microsoft.AspNetCore.Components;
using Xzy.KnowledgeBase.Domain.Repositories;
using Xzy.KnowledgeBase.Models;
using Xzy.KnowledgeBase.Services;

namespace Xzy.KnowledgeBase.Pages
{
    public partial class KmsList
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

        private Kmss [] _data = { };

        [Inject] 
        protected IKmss_Repositories _kmss_Repositories { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var list = new List<Kmss> { new Kmss() };
            var data = await _kmss_Repositories.GetListAsync();
            list.AddRange(data);
            _data = list.ToArray();
        }
    }
}

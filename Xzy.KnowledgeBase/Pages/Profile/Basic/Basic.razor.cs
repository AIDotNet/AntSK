using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Xzy.KnowledgeBase.Models;
using Xzy.KnowledgeBase.Services;

namespace Xzy.KnowledgeBase.Pages.Profile
{
    public partial class Basic
    {
        private BasicProfileDataType _data = new BasicProfileDataType();

        [Inject] protected IProfileService ProfileService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _data = await ProfileService.GetBasicAsync();
        }
    }
}
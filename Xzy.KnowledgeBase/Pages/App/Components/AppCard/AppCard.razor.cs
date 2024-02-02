using Microsoft.AspNetCore.Components;

namespace Xzy.KnowledgeBase.Pages
{
    public partial class AppCard
    {

        [Parameter]
        public string Avatar { get; set; }

        [Parameter]
        public string Title { get; set; }


        [Parameter]
        public string Desc { get; set; }


        [Parameter]
        public RenderFragment Footer { get; set; }

        [Parameter]
        public string ContentHeight { get; set; }
    }
}

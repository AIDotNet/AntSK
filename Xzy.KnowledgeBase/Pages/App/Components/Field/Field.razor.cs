using Microsoft.AspNetCore.Components;

namespace Xzy.KnowledgeBase.Pages
{
    public partial class Field
    {
        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string Value { get; set; }
    }
}
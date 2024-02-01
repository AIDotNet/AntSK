using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using Xzy.KnowledgeBase.Models;

namespace Xzy.KnowledgeBase.Pages.Account.Center
{
    public partial class Articles
    {
        [Parameter] public IList<ListItemDataType> List { get; set; }
    }
}
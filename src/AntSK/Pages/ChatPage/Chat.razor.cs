using AntDesign;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;
using Markdig;
using AntSK.Domain.Domain.Model;
using AntSK.Domain.Domain.Model.Dto;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AntSK.Pages.ChatPage
{
    public partial class Chat
    {
        [Parameter] public string AppId { get; set; }

        [Inject] protected IApps_Repositories _apps_Repositories { get; set; }
      
        private List<RelevantSource> _relevantSources = new List<RelevantSource>();

        protected List<Apps> _list = new List<Apps>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _list = _apps_Repositories.GetList();
        }

        private void OnRelevantSources(List<RelevantSource> relevantSources)
        {
            _relevantSources = relevantSources;
            InvokeAsync(StateHasChanged);
        }
    }
}
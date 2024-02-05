using AntSK.Domain.Domain.Dto;
using AntSK.Domain.Domain.Interface;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Components;

namespace AntSK.Pages.Kms
{
    public partial class KmsDetailList
    {
        [Parameter]
        public string KmsId { get; set; }
        [Parameter]
        public string FileId { get; set; }

        [Inject]
        protected IKMService iKMService { get; set; }

        private List<KMFile> _data = new List<KMFile>() ;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _data = await iKMService.GetDocumentByFileID(FileId);
        }

        private void NavigateBack() {
            NavigationManager.NavigateTo($"/kms/detail/{KmsId}");
        } 
    }
}

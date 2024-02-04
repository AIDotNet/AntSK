using AntDesign;
using AntSK.Domain.Repositories;
using AntSK.Models;
using AntSK.Services;
using Microsoft.AspNetCore.Components;

namespace AntSK.Pages.Kms
{
    public partial class KmsDetail
    {
        [Parameter]
        public string KmsId { get; set; }

        private readonly KmsDetails _model = new KmsDetails();

        private readonly IDictionary<string, ProgressStatus> _pStatus = new Dictionary<string, ProgressStatus>
        {
            {"active", ProgressStatus.Active},
            {"exception", ProgressStatus.Exception},
            {"normal", ProgressStatus.Normal},
            {"success", ProgressStatus.Success}
        };

        private List<KmsDetails> _data =new List<KmsDetails>();

        [Inject]
        protected IKmsDetails_Repositories _kmsDetails_Repositories { get; set; }


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _data =await _kmsDetails_Repositories.GetListAsync(p => p.KmsId == KmsId);
        }
    }
}

using AntDesign;
using AntSK.Domain.Repositories;
using AntSK.Models;
using AntSK.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.KernelMemory;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace AntSK.Pages.Kms
{
    public partial class KmsDetail
    {
        [Parameter]
        public string KmsId { get; set; }

        private readonly KmsDetails _model = new KmsDetails();

        bool _urlVisible = false;

        private Form<UrlModel> _urlForm;
        private UrlModel urlModel = new UrlModel();

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
        [Inject]
        protected MemoryServerless _memory { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _data =await _kmsDetails_Repositories.GetListAsync(p => p.KmsId == KmsId);
        }

        private async Task FileUpload()
        {
           
        }

        public class UrlModel
        {
            [Required]
            public string Url { get; set; }
        }
        private async Task UrlHandleOk(MouseEventArgs e)
        {
            string fileid = Guid.NewGuid().ToString();
            await _memory.ImportWebPageAsync(urlModel.Url, fileid, new TagCollection() { { "kmsid", KmsId } }
                 , index: "kms");
            KmsDetails detial = new KmsDetails()
            {
                Id = fileid,
                KmsId = KmsId,
                Type = "url",
                Url = urlModel.Url
            };
            await _kmsDetails_Repositories.InsertAsync(detial);
            _data = await _kmsDetails_Repositories.GetListAsync(p => p.KmsId == KmsId);
        }

        private void ShowUrlModal()
        {
            _urlVisible = true;
        }
    }
}

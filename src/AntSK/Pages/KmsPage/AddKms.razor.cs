using AntDesign;
using AntSK.Domain.Model.Enum;
using AntSK.Domain.Repositories;
using Microsoft.AspNetCore.Components;

namespace AntSK.Pages.KmsPage
{

    public partial class AddKms
    {
        [Parameter]
        public string KmsId { get; set; }
        [Inject]
        protected IKmss_Repositories _kmss_Repositories { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        protected MessageService? Message { get; set; }

        [Inject]
        protected IAIModels_Repositories _aimodels_Repositories { get; set; }


        private Kmss _kmsModel = new Kmss();

        private List<AIModels> _chatList { get; set; }
        private List<AIModels> _embeddingList { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _chatList = _aimodels_Repositories.GetList(p => p.AIModelType == AIModelType.Chat);
            _embeddingList = _aimodels_Repositories.GetList(p => p.AIModelType == AIModelType.Embedding);
            if (!string.IsNullOrEmpty(KmsId))
            {
                //查看
                _kmsModel = await _kmss_Repositories.GetFirstAsync(p => p.Id == KmsId);
            }
        }
        private void HandleSubmit()
        {
            if (_kmsModel.OverlappingTokens >= _kmsModel.MaxTokensPerLine || _kmsModel.OverlappingTokens >= _kmsModel.MaxTokensPerParagraph)
            {
                _ = Message.Error("重叠部分需小于行切片和段落切片！", 2);
                return;
            }

            if (_kmsModel.MaxTokensPerLine >= _kmsModel.MaxTokensPerParagraph)
            {
                _ = Message.Error("行切片需小于段落切片！", 2);
                return;
            }

            if (string.IsNullOrEmpty(KmsId))
            {
                _kmsModel.Id = Guid.NewGuid().ToString();
                if (_kmss_Repositories.IsAny(p => p.Name == _kmsModel.Name))
                {
                    _ = Message.Error("名称已存在！", 2);
                    return;
                }
                _kmss_Repositories.Insert(_kmsModel);
            }
            else
            {
                _kmss_Repositories.Update(_kmsModel);
            }

            NavigationManager.NavigateTo("/kmslist");

        }

        private void NavigateModelList()
        {
            NavigationManager.NavigateTo("/setting/modellist");
        }
    }
}

using AntDesign;
using AntSK.Domain.Domain.Model.Enum;
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

        private bool isOcr { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //星火 Mock没实现KM先隐藏
            List<AIType> ignores = new List<AIType>() { AIType.SparkDesk, AIType.Mock };
            _chatList = _aimodels_Repositories.GetList(p => p.AIModelType == AIModelType.Chat&& !ignores.Contains(p.AIType));
            _embeddingList = _aimodels_Repositories.GetList(p => p.AIModelType == AIModelType.Embedding && !ignores.Contains(p.AIType));
            if (!string.IsNullOrEmpty(KmsId))
            {
                //查看
                _kmsModel = await _kmss_Repositories.GetFirstAsync(p => p.Id == KmsId);
                isOcr = _kmsModel.IsOCR == 1;
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
            if (isOcr)
            {
                _kmsModel.IsOCR = 1;
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
            NavigationManager.NavigateTo("/modelmanager/modellist");
        }
    }
}

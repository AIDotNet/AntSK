using AntDesign;
using AntSK.Models;
using AntSK.Services;
using Microsoft.AspNetCore.Components;

namespace AntSK.Pages.Setting.AIModel
{
    public partial class ModelDown
    {
        private readonly ListFormModel _model = new ListFormModel();
        private readonly IList<string> _selectCategories = new List<string>();

        private IList<ListItemDataType> _fakeList = new List<ListItemDataType>();


        [Inject] public IProjectService ProjectService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _fakeList = await ProjectService.GetFakeListAsync(8);
        }

        private async Task Search(string searchKey)
        {
            
        }
    }
}

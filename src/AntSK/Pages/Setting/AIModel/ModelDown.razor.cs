using AntDesign;
using AntSK.Models;
using AntSK.Services;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using RestSharp;
using AntSK.Domain.Utils;
using AntSK.Domain.Domain.Model.hfmirror;

namespace AntSK.Pages.Setting.AIModel
{
    public partial class ModelDown
    {
        private readonly ListFormModel _model = new ListFormModel();
        private readonly IList<string> _selectCategories = new List<string>();

        private List<HfModels> _modelList = new List<HfModels>();
        private string _modelType;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            InitData("");
        }

        private void InitData(string searchKey)
        {
            var param = searchKey.ConvertToString().Split(" ");

            string urlBase = $"https://hf-mirror.com/models-json?sort=trending&search={_modelType}";
            if (param.Count() > 0)
            {
                urlBase += "+" + string.Join("+", param);
            }
            RestClient client = new RestClient();
            RestRequest request = new RestRequest(urlBase, Method.Get);
            var response = client.Execute(request);
            var model = JsonConvert.DeserializeObject<HfModel>(response.Content);
            _modelList = model.models;
        }

        private async Task Search(string searchKey)
        {
            InitData(searchKey);
        }

        private void Down(string modelPath)
        {
            NavigationManager.NavigateTo($"/setting/modeldown/detail/{modelPath}");
        }

        private void OnModelTypeChange(string value)
        {
            InitData("");
        }
    }
}

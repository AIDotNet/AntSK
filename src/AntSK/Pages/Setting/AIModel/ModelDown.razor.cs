using AntSK.Domain.Domain.Model.hfmirror;
using AntSK.Domain.Utils;
using AntSK.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using RestSharp;

namespace AntSK.Pages.Setting.AIModel
{
    public partial class ModelDown
    {
        private readonly ListFormModel _model = new ListFormModel();
        private readonly IList<string> _selectCategories = new List<string>();

        private List<HfModels> _modelList = new List<HfModels>();
        private string _modelType="gguf";
        private bool loaddding = false;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            InitData("");
        }

        private async Task InitData(string searchKey)
        {
            loaddding = true;
            if (_modelType.Contains("safetensors"))
            {
                _modelList.Clear();
                var param = searchKey.ConvertToString().Split(" ");
                string[] lines = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "StableDiffusionModelList.txt"));
                foreach (string line in lines)
                {
                    string urlBase = $"https://hf-mirror.com/models-json?sort=trending&search={line}";
                    if (param.Count() > 0)
                    {
                        urlBase += "+" + string.Join("+", param);
                    }
                    RestClient client = new RestClient();
                    RestRequest request = new RestRequest(urlBase, Method.Get);
                    var response = await client.ExecuteAsync(request);
                    var model = JsonConvert.DeserializeObject<HfModel>(response.Content);
                    _modelList.AddRange(model.models);
                }

            }
            else
            {
                var param = searchKey.ConvertToString().Split(" ");

                string urlBase = $"https://hf-mirror.com/models-json?sort=trending&search={_modelType}";
                if (param.Count() > 0)
                {
                    urlBase += "+" + string.Join("+", param);
                }
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(urlBase, Method.Get);
                var response = await client.ExecuteAsync(request);
                var model = JsonConvert.DeserializeObject<HfModel>(response.Content);
                _modelList = model.models;
            }

            loaddding = false;
            InvokeAsync(StateHasChanged);
        }

        private async Task Search(string searchKey)
        {
            InitData(searchKey);
        }

        private void Down(string modelPath)
        {
            NavigationManager.NavigateTo($"/modelmanager/modeldown/detail/{modelPath}");
        }

        private void OnModelTypeChange(string value)
        {
            InitData("");
        }
    }
}

using AntSK.Domain.Domain.Model.hfmirror;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Components;
using RestSharp;

namespace AntSK.Pages.Setting.AIModel
{
    public partial class ModelDownDetail
    {
        [Parameter]
        public string ModelName { get; set; }
        [Parameter]
        public string ModelPath { get; set; }

        List<HfModelDetail> modelList = new List<HfModelDetail>();
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            InitData();
        }

        private void InitData()
        {
            string urlBase = $"https://hf-mirror.com/{ModelName}/{ModelPath}/tree/main";
            RestClient client = new RestClient();
            RestRequest request = new RestRequest(urlBase, Method.Get);
            var response = client.Execute(request);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response.Content);

            foreach (var listItem in htmlDocument.DocumentNode.SelectNodes("//li[contains(@class,'grid')]"))
            {
                var modelNameNode = listItem.SelectSingleNode(".//span[contains(@class,'truncate')]");
                var fileSizeNode = listItem.SelectSingleNode(".//a[contains(@class,'text-[0.8rem]')]");
                var downloadNode = listItem.SelectSingleNode(".//a[@title='Download file']");
                var timeNode = listItem.SelectSingleNode(".//time");

                var modelName = modelNameNode?.InnerText.Trim();
                var fileSizeInfo = fileSizeNode?.InnerText.Trim().Split(' ');
                var fileSize = fileSizeInfo?.Length > 1 ? fileSizeInfo[fileSizeInfo.Length - 2] : null;
                var downloadUrl = downloadNode?.GetAttributeValue("href", null)?.Trim();
                var time= timeNode?.InnerText.Trim();

                modelList.Add(new HfModelDetail() { Name = modelName, Size = string.Join(" ",fileSizeInfo), Path = downloadUrl,Time=time });
            }

        }

        private void Down(string path)
        {
            NavigationManager.NavigateTo($"/modelmanager/model/addbypath/{path.Replace("?download=true", "").Replace("/", "---")}");
        }
    }
}

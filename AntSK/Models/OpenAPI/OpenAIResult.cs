using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace AntSK.Models.OpenAPI
{
    public class OpenAIResult
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        [JsonProperty("object")]
        public string obj { get; set; } = "chat.completion";
        public List<ChoicesModel> choices { get; set; }
        public long created { get; set; }
    }

    public class ChoicesModel
    {
        public string finish_reason { get; set; } = "stop";
        public int index { get; set; } = 0;

        public OpenAIMessage message { get; set; }
    }

    public class OpenAIEmbeddingResult 
    {
        [JsonProperty("object")]
        public string obj { get; set; } = "list";
        public string model { get; set; } = "ada";

        public UsageModel usage { get; set; } = new UsageModel();

        public  List<DataModel> data { get; set; } = new List<DataModel>() { new DataModel() };
    }

    public class UsageModel 
    {
        public long prompt_tokens { get; set; } = 0;

        public long total_tokens { get; set; } = 0;
    }

    public class DataModel 
    {
        [JsonProperty("object")]
        public string obj { get; set; } = "embedding";
        public int index { get; set; } = 0;

        public List<float> embedding { get; set; }
    }
}

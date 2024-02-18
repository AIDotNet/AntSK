using System.Text.Json.Serialization;

namespace AntSK.Models.OpenAPI
{
    public class OpenAIResult
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        [JsonPropertyName("object")]
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


}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Models
{
    public class OpenAIModel
    {
        public bool stream { get; set; } = false;
        public List<OpenAIMessage> messages { get; set; }
    }

    public class OpenAIMessage
    {
        public string role { get; set; }

        public string content { get; set; }
    }

    public class OpenAIEmbeddingModel
    {
        public List<string> input { get; set; }
    }

}

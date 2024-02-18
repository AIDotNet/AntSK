using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Model
{
    public class OpenAIModel
    {
       public bool stream { get; set; }
        public List<OpenAIMessage> messages { get; set; } = new List<OpenAIMessage>();
    }

    public class OpenAIMessage
    {
        public string role { get; set; } = "user";

        public string content { get; set; } = "你好";
    }
}

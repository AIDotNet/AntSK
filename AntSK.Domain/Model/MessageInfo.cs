using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Model
{
    public class MessageInfo
    {
        public string ID { get; set; } = "";
        public string Questions { get; set; } = "";
        public string Answers { get; set; } = "";
        public string HtmlAnswers { get; set; } = "";
        public DateTime CreateTime { get; set; }

    }
}

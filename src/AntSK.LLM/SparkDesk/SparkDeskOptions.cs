using Sdcb.SparkDesk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.LLM.SparkDesk
{
    public class SparkDeskOptions
    {
        public string AppId { get; set; }

        public string ApiKey { get; set; }

        public string ApiSecret { get; set; }

        public ModelVersion ModelVersion { get; set; }
    }
}

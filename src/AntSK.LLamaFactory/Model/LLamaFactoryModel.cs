using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.LLamaFactory.Model
{
    public class ModelInfo
    {
        public string DEFAULT { get; set; }
        public string MODELSCOPE { get; set; }
    }

    public class Model
    {
        public ModelInfo DownloadSource { get; set; }
    }

    public class LLamaFactoryModel
    {
        public Dictionary<string, Model> Models { get; set; }
        public string Template { get; set; }
    }
}

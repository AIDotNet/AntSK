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


    public class LLamaFactoryModel
    {
        public Dictionary<string, ModelInfo> Models { get; set; }
        public string Template { get; set; }
    }

    public class LLamaModel
    { 
        public string Name { get; set; }
        public string ModelScope { get; set; }
    }
}

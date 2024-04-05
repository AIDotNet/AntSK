using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Model.Excel
{
    public class KMSExcelModel
    {
        [ExeclProperty("问题",0)]
        public string Question { get; set; }

        [ExeclProperty("答案", 1)]
        public string Answer { get; set; }
    }
}

using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntSK.Domain
{
    public class ExeclPropertyAttribute : Attribute
    {
        public ExeclPropertyAttribute()
        {

        }
        public ExeclPropertyAttribute(string displayName, int order, CellType cellType = CellType.String)
        {
            DisplayName = displayName;
            Order = order;
            CellType = cellType;
        }

        public string DisplayName { get; set; }

        public int Order { get; set; }

        public CellType CellType { get; set; }
    }
}

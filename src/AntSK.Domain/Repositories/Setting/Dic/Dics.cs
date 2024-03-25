using AntSK.Domain.Domain.Model.Enum;
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace AntSK.Domain.Repositories
{
    [SugarTable("Dics")]
    public partial class Dics
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }       
        public string Type { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}

using AntSK.Domain.Domain.Model.Enum;
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace AntSK.Domain.Repositories
{
    [SugarTable("Funs")]
    public partial class Funs
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 接口描述
        /// </summary>
        [Required]
        public string Path { get; set; }
      
    }
}

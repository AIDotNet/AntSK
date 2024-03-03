using AntSK.Domain.Model.Enum;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Repositories
{
    [SugarTable("AIModels")]
    public partial class AIModels
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        /// <summary>
        /// 模型类型
        /// </summary>
        [Required]
        public AIModelType AIModelType { get; set; }
        /// <summary>
        /// 模型地址
        /// </summary>
        [Required]
        public string EndPoint { get; set; }
        /// <summary>
        /// 模型名称
        /// </summary>
        [Required]
        public string ModelName { get; set; }
        /// <summary>
        /// 模型秘钥
        /// </summary>
        [Required]
        public string ModelKey { get; set; }
        [Required]
        public string ModelDescription { get; set; }
    }
}

using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Repositories
{
    [SugarTable("Apps")]
    public partial class Apps
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Required]
        public string Describe { get; set; }
        [Required]
        public string Icon { get; set; }

        [Required]
        public string Type { get; set; }

        /// <summary>
        /// 提示词
        /// </summary>
        public string? Prompt { get; set; }
        /// <summary>
        /// 知识库ID列表
        /// </summary>
        public string? KmsIdList { get; set; }
    }
}

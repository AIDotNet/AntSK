using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace AntSK.Domain.Repositories
{
    /// <summary>
    /// 角色表
    /// </summary>
    [SugarTable("Roles")]
    public partial class Roles
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        [Required]
        public string Code { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}

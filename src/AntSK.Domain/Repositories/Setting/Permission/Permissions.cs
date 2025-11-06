using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace AntSK.Domain.Repositories
{
    /// <summary>
    /// 权限表
    /// </summary>
    [SugarTable("Permissions")]
    public partial class Permissions
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 权限编码
        /// </summary>
        [Required]
        public string Code { get; set; }

        /// <summary>
        /// 权限类型（Menu-菜单权限, Operation-操作权限）
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}

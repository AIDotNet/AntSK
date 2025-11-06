using SqlSugar;

namespace AntSK.Domain.Repositories
{
    /// <summary>
    /// 角色权限关联表
    /// </summary>
    [SugarTable("RolePermissions")]
    public partial class RolePermissions
    {
        /// <summary>
        /// 关联ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        public string PermissionId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}

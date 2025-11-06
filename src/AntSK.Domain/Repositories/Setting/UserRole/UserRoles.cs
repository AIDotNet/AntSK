using SqlSugar;

namespace AntSK.Domain.Repositories
{
    /// <summary>
    /// 用户角色关联表
    /// </summary>
    [SugarTable("UserRoles")]
    public partial class UserRoles
    {
        /// <summary>
        /// 关联ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}

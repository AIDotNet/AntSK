using SqlSugar;
using System.ComponentModel.DataAnnotations;

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

        /// <summary>
        /// 图标
        /// </summary>
        [Required]
        public string Icon { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// 会话模型ID
        /// </summary>
        [Required]
        public string? ChatModelID { get; set; }

        /// <summary>
        /// Embedding 模型Id
        /// </summary>
        public string? EmbeddingModelID { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        [SugarColumn(DefaultValue = "70")]
        public double Temperature { get; set; } = 70f;

        /// <summary>
        /// 提示词
        /// </summary>
        public string? Prompt { get; set; }

        /// <summary>
        /// 插件列表
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar(1000)")]
        public string? ApiFunctionList { get; set; }

        /// <summary>
        /// 本地函数列表
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar(1000)")]
        public string? NativeFunctionList { get; set; }

        /// <summary>
        /// 知识库ID列表
        /// </summary>
        public string? KmsIdList { get; set; }

        /// <summary>
        /// API调用秘钥
        /// </summary>
        public string? SecretKey { get; set; }
    }
}
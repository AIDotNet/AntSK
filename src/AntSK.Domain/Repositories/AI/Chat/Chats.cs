using AntSK.Domain.Domain.Model.Enum;
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace AntSK.Domain.Repositories
{
    [SugarTable("Chats")]
    public partial class Chats
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar(4000)")]
        public string Context { get; set; } = "";

        /// <summary>
        /// 发送是true  接收是false
        /// </summary>
        public bool IsSend { get; set; } = false;
        /// <summary>
        /// 创建事件
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string? FileName { get; set; }
    }
}

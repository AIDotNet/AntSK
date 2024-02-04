using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Repositories
{
    [SugarTable("KmsDetails")]
    public partial class KmsDetails
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        public string KmsId { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 类型 file，url
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 数据数量
        /// </summary>
        public int DataCount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}

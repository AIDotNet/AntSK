using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Repositories
{
    [SugarTable("Kms")]
    public partial class Kmss
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 会话模型
        /// </summary>
        public string ChatModel { get; set; }
        /// <summary>
        /// 向量模型
        /// </summary>
        public string EmbeddingModel { get; set; }
    }
}

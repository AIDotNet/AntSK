using AntSK.Domain.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Repositories
{
    [SugarTable("Apis")]
    public partial class Apis
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 接口名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 接口描述
        /// </summary>
        [Required]
        public string Describe { get; set; }
        /// <summary>
        /// 接口地址
        /// </summary>
        [Required]
        public string Url { get; set; }
        /// <summary>
        /// 请求方法
        /// </summary>
        [Required]
        public HttpMethodType Method { get; set; }
        /// <summary>
        /// 入参提示词
        /// </summary>
        [Required]
        public string InputPrompt { get; set; }

        /// <summary>
        /// 返回提示词
        /// </summary>
        public string OutputPrompt { get; set; }
        /// <summary>
        /// json 实体
        /// </summary>
        public string JsonBody { get; set; }
    }
}

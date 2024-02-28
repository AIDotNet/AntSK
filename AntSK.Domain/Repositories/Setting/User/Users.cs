using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Repositories
{
    [SugarTable("Users")]
    public partial class Users
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 工号，用于登陆
        /// </summary>
        [Required]
        public string No { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Required]
        public string Describe { get; set; }

    }
}

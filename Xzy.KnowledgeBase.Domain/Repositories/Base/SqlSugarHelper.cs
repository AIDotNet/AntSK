using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xzy.KnowledgeBase.Domain.Options;
using Xzy.KnowledgeBase.Domain.Utils;

namespace Xzy.KnowledgeBase.Domain.Repositories.Base
{
    public class SqlSugarHelper
    {
        /// <summary>
        /// sqlserver连接
        /// </summary>
        public static SqlSugarScope Sqlite = new SqlSugarScope(new ConnectionConfig()
        {
            ConnectionString = ConnectionOption.Sqlite,
            DbType = DbType.Sqlite,
            InitKeyType = InitKeyType.Attribute,//从特性读取主键和自增列信息
            IsAutoCloseConnection = true
        }, Db =>
        {
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ConvertToString() == "Development")
                {
                    Console.WriteLine(sql + "\r\n" +
                        Sqlite.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                    Console.WriteLine();
                }
            };
        });
    }
}

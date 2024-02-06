using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntSK.Domain.Options;
using AntSK.Domain.Utils;
using System.Reflection;

namespace AntSK.Domain.Repositories.Base
{
    public class SqlSugarHelper
    {
        /// <summary>
        /// sqlserver连接
        /// </summary>
        public static SqlSugarScope Sqlite = new SqlSugarScope(new ConnectionConfig()
        {
            ConnectionString = ConnectionOption.Postgres,
            DbType = DbType.PostgreSQL,
            InitKeyType = InitKeyType.Attribute,//从特性读取主键和自增列信息
            IsAutoCloseConnection = true,
            ConfigureExternalServices = new ConfigureExternalServices
            {
                //注意:  这儿AOP设置不能少
                EntityService = (c, p) =>
                {
                    /***高版C#写法***/
                    //支持string?和string  
                    if (p.IsPrimarykey == false && new NullabilityInfoContext()
                     .Create(c).WriteState is NullabilityState.Nullable)
                    {
                        p.IsNullable = true;
                    }
                }
            }
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

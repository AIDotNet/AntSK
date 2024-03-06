using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntSK.Domain.Options;
using AntSK.Domain.Utils;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace AntSK.Domain.Repositories.Base
{
    public class SqlSugarHelper()
    {

        /// <summary>
        /// sqlserver连接
        /// </summary>
        public static SqlSugarScope SqlScope() {

            string DBType = DBConnectionOption.DbType;
            string ConnectionString = DBConnectionOption.ConnectionStrings;

            var config = new ConnectionConfig()
            {
                ConnectionString = ConnectionString,
               // DbType = DbType.PostgreSQL,
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
            };
            switch (DBType)
            {
                case "Postgre":
                    config.DbType = DbType.PostgreSQL;
                    break;
                case "Sqlite":
                    config.DbType = DbType.Sqlite;
                    break;
                case "SqlServer":
                    config.DbType = DbType.SqlServer;
                    break;
                case "Mysql":
                    config.DbType = DbType.MySql;
                    break;
            }

            var scope= new SqlSugarScope(config, Db =>
            {
               
            });
            return scope;
        } 
    }
}

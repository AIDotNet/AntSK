using AntSK.Domain.Options;
using SqlSugar;
using System.Reflection;

namespace AntSK.Domain.Repositories.Base
{
    public class SqlSugarHelper()
    {

        /// <summary>
        /// sqlserver连接
        /// </summary>
        public static SqlSugarScope SqlScope()
        {

            string DBType = DBConnectionOption.DbType;
            string ConnectionString = DBConnectionOption.ConnectionStrings;

            var config = new ConnectionConfig()
            {
                ConnectionString = ConnectionString,
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
            DbType dbType = (DbType)Enum.Parse(typeof(DbType), DBType);
            config.DbType = dbType;
            var scope = new SqlSugarScope(config, Db =>
            {

            });
            return scope;
        }
    }
}

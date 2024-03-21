using AntSK.Domain.Domain.Service;
using AntSK.Domain.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Common.DependencyInjection
{
    public static class InitExtensions
    {
        /// <summary>
        /// 使用codefirst创建数据库表
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static WebApplication CodeFirst(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                // 获取仓储服务
                var _repository = scope.ServiceProvider.GetRequiredService<IApps_Repositories>();

                // 创建数据库（如果不存在）
                _repository.GetDB().DbMaintenance.CreateDatabase();

                // 获取当前应用程序域中所有程序集
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                // 在所有程序集中查找具有[SugarTable]特性的类
                foreach (var assembly in assemblies)
                {
                    // 获取该程序集中所有具有SugarTable特性的类型
                    var entityTypes = assembly.GetTypes()
                        .Where(type => TypeIsEntity(type));

                    // 为每个找到的类型初始化数据库表
                    foreach (var type in entityTypes)
                    {
                        _repository.GetDB().CodeFirst.InitTables(type);
                    }
                }
            }
            return app;
        }


        /// <summary>
        /// 加载数据库的插件
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static WebApplication LoadFun(this WebApplication app)
        {
            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    //codefirst 创建表
                    var funRep = scope.ServiceProvider.GetRequiredService<IFuns_Repositories>();
                    var functionService = scope.ServiceProvider.GetRequiredService<FunctionService>();
                    var funs = funRep.GetList();
                    foreach (var fun in funs)
                    {
                        functionService.FuncLoad(fun.Path);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " ---- " + ex.StackTrace);
            }
            return app;
        }
        private static bool TypeIsEntity(Type type)
        {
            // 检查类型是否具有SugarTable特性
            return type.GetCustomAttributes(typeof(SugarTable), inherit: false).Length > 0;
        }
    }
}

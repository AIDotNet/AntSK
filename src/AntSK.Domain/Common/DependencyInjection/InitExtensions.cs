using AntSK.Domain.Domain.Model.Constant;
using AntSK.Domain.Domain.Service;
using AntSK.Domain.Repositories;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SqlSugar;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static WebApplication InitDbData(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                // 初始化字典
                var _dic_Repository = scope.ServiceProvider.GetRequiredService<IDics_Repositories>();
                var llamafactoryStart = _dic_Repository.GetFirst(p => p.Type == LLamaFactoryConstantcs.LLamaFactorDic && p.Key == LLamaFactoryConstantcs.IsStartKey);
                if (llamafactoryStart==null)
                {
                    llamafactoryStart = new Dics();
                    llamafactoryStart.Id=Guid.NewGuid().ToString();
                    llamafactoryStart.Type = LLamaFactoryConstantcs.LLamaFactorDic;
                    llamafactoryStart.Key = LLamaFactoryConstantcs.IsStartKey;
                    llamafactoryStart.Value = "false";
                    _dic_Repository.Insert(llamafactoryStart);
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

        /// <summary>
        /// swagger 初始化
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddAntSKSwagger(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "AntSK.Api", Version = "v1" });
                //添加Api层注释（true表示显示控制器注释）
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, true);
                //添加Domain层注释（true表示显示控制器注释）
                var xmlFile1 = $"{Assembly.GetExecutingAssembly().GetName().Name.Replace("Api", "Domain")}.xml";
                var xmlPath1 = Path.Combine(AppContext.BaseDirectory, xmlFile1);
                c.IncludeXmlComments(xmlPath1, true);
                c.DocInclusionPredicate((docName, apiDes) =>
                {
                    if (!apiDes.TryGetMethodInfo(out MethodInfo method))
                        return false;
                    var version = method.DeclaringType.GetCustomAttributes(true).OfType<ApiExplorerSettingsAttribute>().Select(m => m.GroupName);
                    if (docName == "v1" && !version.Any())
                        return true;
                    var actionVersion = method.GetCustomAttributes(true).OfType<ApiExplorerSettingsAttribute>().Select(m => m.GroupName);
                    if (actionVersion.Any())
                        return actionVersion.Any(v => v == docName);
                    return version.Any(v => v == docName);
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "Directly enter bearer {token} in the box below (note that there is a space between bearer and token)",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference()
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        }, Array.Empty<string>()
                    }
                });
            });
            return serviceCollection;
        }
    }
}

using AntSK.Domain.Domain.Model.Constant;
using AntSK.Domain.Domain.Service;
using AntSK.Domain.Repositories;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        private static ILogger _logger;

        public static void InitLog(ILogger logger)
        {
            _logger = logger;
        }
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
                //安装向量插件
                _repository.GetDB().Ado.ExecuteCommandAsync($"CREATE EXTENSION IF NOT EXISTS vector;");

                _logger.LogInformation("初始化表结构完成");
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

                // 初始化角色和权限
                InitRolesAndPermissions(scope.ServiceProvider);

                _logger.LogInformation("初始化数据库初始数据完成");
            }
            return app;
        }

        private static void InitRolesAndPermissions(IServiceProvider serviceProvider)
        {
            var _roles_Repository = serviceProvider.GetRequiredService<IRoles_Repositories>();
            var _permissions_Repository = serviceProvider.GetRequiredService<IPermissions_Repositories>();
            var _rolePermissions_Repository = serviceProvider.GetRequiredService<IRolePermissions_Repositories>();

            // 检查是否已经初始化
            if (_roles_Repository.IsAny(r => r.Code == "AntSKAdmin"))
            {
                return;
            }

            // 创建管理员角色
            var adminRole = new Roles
            {
                Id = Guid.NewGuid().ToString(),
                Name = "管理员",
                Code = "AntSKAdmin",
                Description = "系统管理员，拥有所有权限",
                IsEnabled = true,
                CreateTime = DateTime.Now
            };
            _roles_Repository.Insert(adminRole);

            // 创建普通用户角色
            var userRole = new Roles
            {
                Id = Guid.NewGuid().ToString(),
                Name = "普通用户",
                Code = "AntSKUser",
                Description = "普通用户，拥有基本功能权限",
                IsEnabled = true,
                CreateTime = DateTime.Now
            };
            _roles_Repository.Insert(userRole);

            // 创建菜单权限
            var menuPermissions = new List<Permissions>
            {
                new Permissions { Id = Guid.NewGuid().ToString(), Name = "聊天", Code = "chat", Type = "Menu", Description = "聊天功能权限" },
                new Permissions { Id = Guid.NewGuid().ToString(), Name = "应用", Code = "app", Type = "Menu", Description = "应用管理权限" },
                new Permissions { Id = Guid.NewGuid().ToString(), Name = "知识库", Code = "kms", Type = "Menu", Description = "知识库管理权限" },
                new Permissions { Id = Guid.NewGuid().ToString(), Name = "API管理", Code = "plugins.apilist", Type = "Menu", Description = "API管理权限" },
                new Permissions { Id = Guid.NewGuid().ToString(), Name = "函数管理", Code = "plugins.funlist", Type = "Menu", Description = "函数管理权限" },
                new Permissions { Id = Guid.NewGuid().ToString(), Name = "模型管理", Code = "modelmanager.modellist", Type = "Menu", Description = "模型管理权限" },
                new Permissions { Id = Guid.NewGuid().ToString(), Name = "用户管理", Code = "setting.user", Type = "Menu", Description = "用户管理权限" },
                new Permissions { Id = Guid.NewGuid().ToString(), Name = "角色管理", Code = "setting.role", Type = "Menu", Description = "角色管理权限" },
                new Permissions { Id = Guid.NewGuid().ToString(), Name = "聊天记录", Code = "setting.chathistory", Type = "Menu", Description = "聊天记录权限" },
                new Permissions { Id = Guid.NewGuid().ToString(), Name = "删除向量表", Code = "setting.delkms", Type = "Menu", Description = "删除向量表权限" }
            };

            foreach (var permission in menuPermissions)
            {
                _permissions_Repository.Insert(permission);
            }

            // 为管理员角色分配所有权限
            foreach (var permission in menuPermissions)
            {
                _rolePermissions_Repository.Insert(new RolePermissions
                {
                    Id = Guid.NewGuid().ToString(),
                    RoleId = adminRole.Id,
                    PermissionId = permission.Id,
                    CreateTime = DateTime.Now
                });
            }

            // 为普通用户角色分配基本权限（聊天、应用、知识库）
            var basicPermissions = menuPermissions.Where(p => p.Code == "chat" || p.Code == "app" || p.Code == "kms").ToList();
            foreach (var permission in basicPermissions)
            {
                _rolePermissions_Repository.Insert(new RolePermissions
                {
                    Id = Guid.NewGuid().ToString(),
                    RoleId = userRole.Id,
                    PermissionId = permission.Id,
                    CreateTime = DateTime.Now
                });
            }

            _logger.LogInformation("初始化角色和权限完成");
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
                _logger.LogError(ex.Message + " ---- " + ex.StackTrace);
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

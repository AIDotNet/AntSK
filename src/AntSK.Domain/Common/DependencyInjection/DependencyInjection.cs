using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Common.DependencyInjection
{
    /// <summary>
    /// 容器扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 从程序集中加载类型并添加到容器中
        /// </summary>
        /// <param name="services">容器</param>
        /// <param name="assemblies">程序集集合</param>
        /// <returns></returns>
        public static IServiceCollection AddServicesFromAssemblies(this IServiceCollection services, params string[] assemblies)
        {
            Type attributeType = typeof(ServiceDescriptionAttribute);
            //var refAssembyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            foreach (var item in assemblies)
            {
                Assembly assembly = Assembly.Load(item);

                var types = assembly.GetTypes();

                foreach (var classType in types)
                {
                    if (!classType.IsAbstract && classType.IsClass && classType.IsDefined(attributeType, false))
                    {
                        ServiceDescriptionAttribute serviceAttribute = classType.GetCustomAttribute(attributeType) as ServiceDescriptionAttribute;
                        switch (serviceAttribute.Lifetime)
                        {
                            case ServiceLifetime.Scoped:
                                services.AddScoped(serviceAttribute.ServiceType, classType);
                                break;

                            case ServiceLifetime.Singleton:
                                services.AddSingleton(serviceAttribute.ServiceType, classType);
                                break;

                            case ServiceLifetime.Transient:
                                services.AddTransient(serviceAttribute.ServiceType, classType);
                                break;
                        }
                    }
                }
            }
            return services;
        }
    }
}

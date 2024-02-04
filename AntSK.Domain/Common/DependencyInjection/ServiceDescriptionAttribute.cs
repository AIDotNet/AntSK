using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Common.DependencyInjection
{
    public class ServiceDescriptionAttribute : Attribute
    {
        public ServiceDescriptionAttribute(Type serviceType, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
        }

        public Type ServiceType { get; set; }

        public ServiceLifetime Lifetime { get; set; }
    }

    public enum ServiceLifetime
    {
        /// <summary>
        /// 作用域
        /// </summary>
        Scoped,
        /// <summary>
        /// 单例
        /// </summary>
        Singleton,
        /// <summary>
        /// 瞬时
        /// </summary>
        Transient
    }
}

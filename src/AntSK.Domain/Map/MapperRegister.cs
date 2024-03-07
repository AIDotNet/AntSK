using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Map
{
    public static class MapperRegister
    {
        public static void AddMapper(this IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMissingTypeMaps = true;
                cfg.ValidateInlineMaps = false;
                cfg.ShouldMapMethod = m => false;
                cfg.AddProfile<AutoMapProfile>();
            });

            IMapper mapper = config.CreateMapper();

            //启动实体映射
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMissingTypeMaps = true;
                cfg.ValidateInlineMaps = false;
                cfg.ShouldMapMethod = m => false;
                cfg.AddProfile<AutoMapProfile>();
            });
        }
    }
}

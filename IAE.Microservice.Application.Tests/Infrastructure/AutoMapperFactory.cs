using AutoMapper;
using IAE.Microservice.Application.Infrastructure.AutoMapper;
using System;

namespace IAE.Microservice.Application.Tests.Infrastructure
{
    public static class AutoMapperFactory
    {
        public static IMapper Create()
        {
            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.ConstructServicesUsing(type => GetResolverByType(type));
                mc.AddProfile(new AutoMapperProfile());
            });

            return mappingConfig.CreateMapper();
        }

        private static object GetResolverByType(Type type) => Activator.CreateInstance(type);
        
    }
}

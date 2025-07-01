using Integrador.Service.Mapper;

namespace Integrador.Configuration
{
    public static class AutoMapperInjectionConfig
    {
        public static void AddDAutoMapperInjectionConfiguration(this IServiceCollection services)
        {
            services.AddAutoMapper(x => x.AddProfile(new MapperProfile()));
        }
    }
}

using Integrador.Ioc.DependecyInjector;

namespace Integrador.Configuration
{
    public static class DependecyInjectorConfig
    {
        public static void AddDependencyInjectionConfiguration(this IServiceCollection services, IConfiguration Configuration)
        {
            DependecyInjector.Register(services, Configuration);
        }
    }
}

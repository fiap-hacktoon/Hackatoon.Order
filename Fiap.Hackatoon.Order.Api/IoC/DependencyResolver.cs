using Fiap.Hackatoon.Order.Application;
using Fiap.Hackatoon.Order.Domain;
using Fiap.Hackatoon.Order.Infrastructure;
using Fiap.Hackatoon.Order.Integrations;

namespace Fiap.Hackatoon.Order.Api.IoC
{
    public static class DependencyResolver
    {
        public static void AddDependencyResolver(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRepositoriesDependency(configuration);
            services.AddDbContextDependency(configuration.GetConnectionString("DefaultConnection"));
            services.AddServicesDependency();
            services.AddApplicationDependency();
            services.AddAuthenticationDependency();
            services.AddMassTransitDependency(configuration);
            services.AddIntegrationsDependency();
        }
    }
}

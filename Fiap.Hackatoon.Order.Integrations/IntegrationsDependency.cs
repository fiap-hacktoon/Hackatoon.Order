using Fiap.Hackatoon.Order.Domain.Interfaces.Integrations;
using Fiap.Hackatoon.Order.Integrations.v1;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.Hackatoon.Order.Integrations
{
    public static class IntegrationsDependency
    {
        public static IServiceCollection AddIntegrationsDependency(this IServiceCollection service)
        {
            service.AddScoped<IProductConsultManager, ProductConsultManager>();

            return service;
        }
    }
}

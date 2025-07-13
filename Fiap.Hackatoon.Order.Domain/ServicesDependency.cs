using Fiap.Hackatoon.Order.Domain.Interfaces.Services;
using Fiap.Hackatoon.Order.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.Hackatoon.Order.Domain
{
    public static class ServicesDependency
    {
        public static IServiceCollection AddServicesDependency(this IServiceCollection service)
        {
            service.AddScoped<IOrderService, OrderService>();
            service.AddScoped<IOrderProductService, OrderProductService>(); 

            return service;
        }
    }
}

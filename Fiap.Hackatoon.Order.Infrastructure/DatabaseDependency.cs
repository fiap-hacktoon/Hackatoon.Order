using Fiap.Hackatoon.Order.Domain.Interfaces.ElasticSearch;
using Fiap.Hackatoon.Order.Domain.Interfaces.Repositories;
using Fiap.Hackatoon.Order.Infrastructure.Data;
using Fiap.Hackatoon.Order.Infrastructure.ElasticSearch;
using Fiap.Hackatoon.Order.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fiap.Hackatoon.Order.Infrastructure
{
    public static class DatabaseDependency
    {
        public static IServiceCollection AddRepositoriesDependency(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddScoped<IOrderRepository, OrderRepository>();
            service.AddScoped<IOrderProductRepository, OrderProductRepository>(); 

            service.Configure<ElasticSettings>(configuration.GetSection("ElasticSettings"));
            service.AddSingleton<IElasticSettings>(sp => sp.GetRequiredService<IOptions<ElasticSettings>>().Value);
            service.AddSingleton(typeof(IElasticClient<>), typeof(ElasticClient<>));

            return service;
        }

        public static IServiceCollection AddDbContextDependency(this IServiceCollection service, string connectionString)
        {
            service.AddDbContext<OrderDbContext>(options => options.UseMySql(connectionString,
                                                            new MySqlServerVersion(new Version(8, 0, 21)),
                                                            mySqlOptions => mySqlOptions.MigrationsAssembly("Fiap.Hackatoon.Order.Infrastructure")));

            return service;
        }
    }
}

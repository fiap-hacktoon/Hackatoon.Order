using Fiap.Hackatoon.Order.Application.Services;
using Fiap.Hackatoon.Order.Domain.Dtos.Application;
using Fiap.Hackatoon.Order.Domain.Interfaces.Application;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Fiap.Hackatoon.Order.Application
{
    public static class ApplicationDependency
    {
        public static IServiceCollection AddApplicationDependency(this IServiceCollection service)
        {
            service.AddScoped<IOrderApplication, OrderApplication>();
            service.AddScoped<ITokenApplication, TokenApplication>();
            return service;
        }

        public static IServiceCollection AddMassTransitDependency(this IServiceCollection service, 
                                                                  IConfiguration configuration)
        {
            var massTransitConfig = new MassTransitDTO()
            {
                QueueList = new Queues
                {
                    InsertQueue = configuration.GetSection("MassTransit:QueueList:InsertQueue").Value ?? string.Empty,

                    UpdateQueue = configuration.GetSection("MassTransit:QueueList:UpdateQueue").Value ?? string.Empty,

                    DeleteQueue = configuration.GetSection("MassTransit:QueueList:DeleteQueue").Value ?? string.Empty,
                },

                Server = configuration.GetSection("MassTransit")["Server"] ?? string.Empty,

                User = configuration.GetSection("MassTransit")["User"] ?? string.Empty,

                Password = configuration.GetSection("MassTransit")["Password"] ?? string.Empty,

                Port = configuration.GetSection("MassTransit")["Port"] ?? string.Empty,
            };

            service.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(massTransitConfig.Server, massTransitConfig.Port, "/", h =>
                    {
                        h.Username(massTransitConfig.User);
                        h.Password(massTransitConfig.Password);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return service;
        }

        public static IServiceCollection AddAuthenticationDependency(this IServiceCollection service)
        {
            var _configuration = new ConfigurationBuilder()
                                 .AddJsonFile("appsettings.json")
                                 .Build();

            var chaveCriptografia = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("SecretJWT") ?? string.Empty);

            service.AddAuthentication(x =>
                   {
                        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                   })
                   .AddJwtBearer(x =>
                   {
                        x.RequireHttpsMetadata = false;
                        x.SaveToken = true;
                        x.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(chaveCriptografia),
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                   });

            return service;
        }
    }
}

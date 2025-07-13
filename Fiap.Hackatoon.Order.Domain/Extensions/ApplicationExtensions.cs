using Elastic.Clients.Elasticsearch.IndexLifecycleManagement;
using Fiap.Hackatoon.Order.Domain.Dtos.Application;
using Microsoft.Extensions.Configuration;

namespace Fiap.Hackatoon.Order.Domain.Extensions
{
    public static class ApplicationExtensions
    {
        public static MassTransitDTO ChargeMassTransitObject(this IConfiguration configuration)
        {
            return new MassTransitDTO()
            {
                QueueList = new Queues
                {
                    InsertQueue = configuration.GetSection("MassTransit:QueueList:InsertQueue").Value ?? string.Empty,

                    UpdateQueue = configuration.GetSection("MassTransit:QueueList:UpdateQueue").Value ?? string.Empty,

                    DeleteQueue = configuration.GetSection("MassTransit:QueueList:DeleteQueue").Value ?? string.Empty,
                },

                Server = configuration.GetSection("MassTransit:Server").Value ?? string.Empty,

                User = configuration.GetSection("MassTransit:User").Value ?? string.Empty,

                Password = configuration.GetSection("MassTransit:Password").Value ?? string.Empty
            };
        }
    }
}

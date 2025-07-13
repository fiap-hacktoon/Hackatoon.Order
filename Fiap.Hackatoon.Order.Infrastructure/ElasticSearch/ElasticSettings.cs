using Fiap.Hackatoon.Order.Domain.Interfaces.ElasticSearch;

namespace Fiap.Hackatoon.Order.Infrastructure.ElasticSearch
{
    public class ElasticSettings : IElasticSettings
    {
        public string ApiKey { get; set; }

        public string CloudId { get; set; }
    }
}

namespace Fiap.Hackatoon.Order.Domain.Interfaces.ElasticSearch
{
    public interface IElasticSettings
    {
        string ApiKey { get; set; }

        string CloudId { get; set; }
    }
}

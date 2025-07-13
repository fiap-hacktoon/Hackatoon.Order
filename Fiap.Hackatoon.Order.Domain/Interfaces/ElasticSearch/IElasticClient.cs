using Elastic.Clients.Elasticsearch;

namespace Fiap.Hackatoon.Order.Domain.Interfaces.ElasticSearch
{
    public interface IElasticClient<T>
    {
        Task<IReadOnlyCollection<T>> Get(int page, int size, IndexName index);

        Task<bool> Create(T log, IndexName index);

        Task<List<T>> GetByStatus(int id, IndexName index);

        Task<List<T>> GetByJsonId(string id, IndexName index);

        Task<List<T>> GetByEmployeeId(int guidEmployee, IndexName index);

        Task<List<T>> GetByClientId(int guidClient, IndexName index);

        Task<bool> Replace(string id, T document, IndexName index);

        Task<bool> Delete(string id, IndexName index);
    }
}

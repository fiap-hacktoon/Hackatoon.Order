using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Fiap.Hackatoon.Order.Domain.Interfaces.ElasticSearch;

namespace Fiap.Hackatoon.Order.Infrastructure.ElasticSearch
{
    public class ElasticClient<T>(IElasticSettings settings) : IElasticClient<T>
    {
        private readonly ElasticsearchClient _client = new(settings.CloudId, new ApiKey(settings.ApiKey));
        
        public async Task<IReadOnlyCollection<T>> Get(int page, int size, IndexName index)
        {
            try
            {
                var response = await _client.SearchAsync<T>(s => s.Index(index)
                                                                  .From(page)
                                                                  .Size(size));
                return response.Documents;
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        public async Task<List<T>> GetByStatus(int id, IndexName index)
        {
            var response = await _client.SearchAsync<T>(s => s
                .Index(index)
                .Query(q => q
                    .Term(t => t
                        .Field("orderStatusId")
                        .Value(id)
                    )
                )
            );

            if (!response.IsValidResponse)
            {
                Console.Error.WriteLine($"Erro na busca por Status: {response.DebugInformation}");
                return [];
            }

            return [.. response.Documents];
        }

        public async Task<List<T>> GetByEmployeeId(int guidEmployee, IndexName index)
        {
            var response = await _client.SearchAsync<T>(s => s
                .Index(index)
                .Query(q => q
                    .Term(t => t
                        .Field("employeeId")
                        .Value(guidEmployee)
                    )
                )
            );

            if (!response.IsValidResponse)
            {
                Console.Error.WriteLine($"Erro na busca por Funcionario: {response.DebugInformation}");
                return [];
            }

            return [.. response.Documents];
        }

        public async Task<List<T>> GetByClientId(int guidClient, IndexName index)
        {
            var response = await _client.SearchAsync<T>(s => s
                .Index(index)
                .Query(q => q
                    .Term(t => t
                        .Field("clientId")
                        .Value(guidClient)
                    )
                )
            );

            if (!response.IsValidResponse)
            {
                Console.Error.WriteLine($"Erro na busca por Cliente: {response.DebugInformation}");
                return [];
            }

            return [.. response.Documents];
        }

        public async Task<List<T>> GetByJsonId(string id, IndexName index)
        {
            var response = await _client.SearchAsync<T>(s => s
                .Index(index)
                .Query(q => q
                    .Term(t => t
                        .Field("id") // certifique-se que é "id" minúsculo se estiver assim no Elasticsearch
                        .Value(id)
                    )
                )
            );

            if (!response.IsValidResponse)
            {
                Console.Error.WriteLine($"Erro na busca por Id: {response.DebugInformation}");
                return [];
            }

            return response.Documents.ToList();
        }

        public async Task<bool> Create(T log, IndexName index)
        {
            var response = await _client.IndexAsync(log, index);

            return response.IsValidResponse;
        }

        public async Task<bool> Replace(string id, T document, IndexName index)
        {
            var response = await _client.IndexAsync(document, i => i
                .Index(index)
                .Id(id)
            );

            return response.IsValidResponse;
        }

        public async Task<bool> Delete(string id, IndexName index)
        {
            var response = await _client.DeleteAsync<T>(id, d => d.Index(index));

            return response.IsValidResponse;
        }
    }
}

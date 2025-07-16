using Fiap.Hackatoon.Order.Domain.Entities;
using Fiap.Hackatoon.Order.Domain.Interfaces.ElasticSearch;
using Fiap.Hackatoon.Order.Domain.Interfaces.Repositories;
using Fiap.Hackatoon.Order.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Fiap.Hackatoon.Order.Domain.Services
{
    public class OrderService (
        IOrderRepository orderRepository,
        ILogger<OrderService> logger,
        IElasticClient<OrderEntity> elasticClient) : IOrderService
    { 
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly ILogger<OrderService> _logger = logger;
        private readonly IElasticClient<OrderEntity> _elasticClient = elasticClient;

        private const string INDEX_NAME = "order";

        private async Task<IReadOnlyCollection<OrderEntity>> GetAllOrdersElastic(int page, int size)
        {
            var documents = await _elasticClient.Get(page, size, INDEX_NAME);
            return documents;
        }

        public async Task<IEnumerable<OrderEntity>> GetAllAsync()
        {
            try
            {                
                var indexedOrders = await GetAllOrdersElastic(0, 10000);
                if (indexedOrders is not null && indexedOrders.Any())
                    return indexedOrders;
                else
                    return await _orderRepository.GetAllAsync();
            }
            catch (Exception e)
            {
                var message = $"Some error occour when trying to get all orders in database.";
                _logger.LogError(message: message, args: e);
                throw new Exception(message);
            }
        }

        public async Task<OrderEntity> GetByIdAsync(string id)
        {
            try
            {
                var orderInxedList = await _elasticClient.GetByJsonId(id, INDEX_NAME);
                if (orderInxedList != null && orderInxedList.Any())
                    return orderInxedList.FirstOrDefault();
                else
                    return await _orderRepository.GetByIdAsync(id);
            }
            catch (Exception)
            {
                var message = $"Some error occour when trying to get a order with Id: {id} Contact.";
                _logger.LogError(message);
                throw new Exception(message);
            }
        }

        public async Task<IEnumerable<OrderEntity>> GetOrdersByStatusIdAsync(int status)
        {
            try
            {
                var orderInxedList = (await _elasticClient.GetByStatus(status, INDEX_NAME));
                if (orderInxedList is not null && orderInxedList.Any())
                    return orderInxedList;
                else                
                    return await _orderRepository.GetOrderByStatusAsync(status);                 
            }
            catch (Exception e)
            {
                var message = $"Some error occour when trying to get a order by status with the Id: {status}.";
                _logger.LogError(message, e);
                throw new Exception(message);
            }
        }

        public async Task<IEnumerable<OrderEntity>> GetOrderByClientIdAsync(int clientId)
        {
            try
            {
                var orderInxedList = (await _elasticClient.GetByClientId(clientId, INDEX_NAME));
                if (orderInxedList is not null && orderInxedList.Any())
                    return orderInxedList;
                else                                    
                    return  await _orderRepository.GetByClientAsync(clientId);                
            }
            catch (Exception e)
            {
                var message = $"Some error occour when trying to get a order by client with the Id: {clientId}.";
                _logger.LogError(message, e);
                throw new Exception(message);
            }
        }

        public async Task<IEnumerable<OrderEntity>> GetOrderByEmployeeIdAsync(int employeeId)
        {
            try
            {
                var orderInxedList = (await _elasticClient.GetByEmployeeId(employeeId, INDEX_NAME));
                if (orderInxedList is not null && orderInxedList.Any())
                    return orderInxedList;
                else                
                    return await _orderRepository.GetByEmployeeAsync(employeeId);                    
            }
            catch (Exception e)
            {
                var message = $"Some error occour when trying to get a order by employee with the Id: {employeeId}.";
                _logger.LogError(message, e);
                throw new Exception(message);
            }
        }
    }
}

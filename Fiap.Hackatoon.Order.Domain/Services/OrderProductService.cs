using Fiap.Hackatoon.Order.Domain.Dtos.Order;
using Fiap.Hackatoon.Order.Domain.Entities;
using Fiap.Hackatoon.Order.Domain.Interfaces.ElasticSearch;
using Fiap.Hackatoon.Order.Domain.Interfaces.Integrations;
using Fiap.Hackatoon.Order.Domain.Interfaces.Repositories;
using Fiap.Hackatoon.Order.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Fiap.Hackatoon.Order.Domain.Services
{
    public class OrderProductService(
        IOrderProductRepository orderProductRepository,
        ILogger<OrderProductService> logger,
        IElasticClient<OrderProduct> elasticClient,
        IProductConsultManager productConsultManager) : IOrderProductService
    {
        private readonly IOrderProductRepository _orderProductRepository = orderProductRepository;
        private readonly ILogger<OrderProductService> _logger = logger;
        private readonly IElasticClient<OrderProduct> _elasticClient = elasticClient;
        private readonly IProductConsultManager _productConsultManager = productConsultManager;

        private const string INDEX_NAME = "order-product";

        public async Task<Product> ValidateProduct(string id)
        {
            try
            {
                var product = await _productConsultManager.GetProductById(id, string.Empty);
                return product;
            }
            catch (Exception ex)
            {
                var message = $"Error while trying to validate a Product with ID: {id}.";
                _logger.LogError(ex, message);
                throw new ApplicationException(message, ex);
            }
        }

        public async Task<OrderProduct> GetByIdAsync(string id)
        {
            try
            {
                var fromElastic = (await _elasticClient.GetByJsonId(id, INDEX_NAME)).FirstOrDefault();
                if (fromElastic != null)
                    return fromElastic;

                return await _orderProductRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                var message = $"Error while retrieving OrderProduct with ID: {id}.";
                _logger.LogError(ex, message);
                throw new ApplicationException(message, ex);
            }
        }

        public async Task<List<OrderProduct?>> GetByOrderIdAsync(string orderId)
        {
            try
            {
                var fromElastic = await _elasticClient.GetByJsonId(orderId, INDEX_NAME);
                if (fromElastic?.Any() == true)
                    return fromElastic;

                return await _orderProductRepository.GetByOrderIdAsync(orderId);
            }
            catch (Exception ex)
            {
                var message = $"Error while retrieving OrderProducts by OrderId: {orderId}.";
                _logger.LogError(ex, message);
                throw new ApplicationException(message, ex);
            }
        }

        public async Task<List<OrderProduct>> GetAllAsync()
        {
            try
            {
                var fromElastic = await _elasticClient.Get(0, 10000, INDEX_NAME);
                if (fromElastic?.Any() == true)
                    return fromElastic.ToList();

                return await _orderProductRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                const string message = "Error while retrieving all OrderProducts.";
                _logger.LogError(ex, message);
                throw new ApplicationException(message, ex);
            }
        }

        public async Task AddAsync(OrderProduct orderProduct)
        {
            try
            {
                // Enriquecimento de dados externo, se necessário
                //await _productConsultManager.EnrichProductData(orderProduct);

                await _orderProductRepository.AddAsync(orderProduct);
                await _elasticClient.Create(orderProduct, INDEX_NAME);
            }
            catch (Exception ex)
            {
                const string message = "Error while adding a new OrderProduct.";
                _logger.LogError(ex, message);
                throw new ApplicationException(message, ex);
            }
        }

        public async Task UpdateAsync(OrderProduct orderProduct)
        {
            try
            {
                //await _productConsultManager.EnrichProductData(orderProduct);

                await _orderProductRepository.UpdateAsync(orderProduct);
                await _elasticClient.Replace(orderProduct.Id.ToString(), orderProduct, INDEX_NAME);
            }
            catch (Exception ex)
            {
                const string message = "Error while updating the OrderProduct.";
                _logger.LogError(ex, message);
                throw new ApplicationException(message, ex);
            }
        }

        public async Task DeleteAsync(OrderProduct orderProduct)
        {
            try
            {
                await _orderProductRepository.DeleteAsync(orderProduct);
                await _elasticClient.Delete(orderProduct.Id.ToString(), INDEX_NAME);
            }
            catch (Exception ex)
            {
                const string message = "Error while deleting the OrderProduct.";
                _logger.LogError(ex, message);
                throw new ApplicationException(message, ex);
            }
        }
    }
}

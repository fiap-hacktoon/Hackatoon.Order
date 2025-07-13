using Elastic.Clients.Elasticsearch;
using Fiap.Hackatoon.Order.Domain.Entities;
using Fiap.Hackatoon.Order.Domain.Interfaces.ElasticSearch;
using Fiap.Hackatoon.Order.Domain.Interfaces.Integrations;
using Fiap.Hackatoon.Order.Domain.Interfaces.Repositories;
using Fiap.Hackatoon.Order.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.Hackatoon.Order.UnitTests.Domain
{
    public class OrderProductServiceTests
    {
        private readonly Mock<IOrderProductRepository> _orderProductRepoMock;
        private readonly Mock<IElasticClient<OrderProduct>> _elasticClientMock;
        private readonly Mock<ILogger<OrderProductService>> _loggerMock;
        private readonly Mock<IProductConsultManager> _productConsultManagerMock;
        private readonly OrderProductService _service;

        public OrderProductServiceTests()
        {
            _orderProductRepoMock = new Mock<IOrderProductRepository>();
            _elasticClientMock = new Mock<IElasticClient<OrderProduct>>();
            _loggerMock = new Mock<ILogger<OrderProductService>>();
            _productConsultManagerMock = new Mock<IProductConsultManager>();

            _service = new OrderProductService(
                _orderProductRepoMock.Object,
                _loggerMock.Object,
                _elasticClientMock.Object,
                _productConsultManagerMock.Object
            );
        }

        [Fact]
        public async Task ValidateProduct_ReturnsProduct_WhenValid()
        {
            var idTeste = Guid.NewGuid();
            var product = new Product { Id = idTeste };

            _productConsultManagerMock.Setup(x => x.GetProductById(It.IsAny<string>(), It.IsAny<string>()))
                                      .ReturnsAsync(product);

            var result = await _service.ValidateProduct(idTeste.ToString());

            Assert.Equal(idTeste, result.Id);
        }

        [Fact]
        public async Task ValidateProduct_ThrowsException_AndLogsError()
        {
            _productConsultManagerMock.Setup(x => x.GetProductById(It.IsAny<string>(), It.IsAny<string>()))
                                      .ThrowsAsync(new Exception("fail"));

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _service.ValidateProduct("P1"));

            Assert.Contains("validate a Product", ex.Message);
            _loggerMock.VerifyLog(LogLevel.Error, Times.Once());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsFromElastic_WhenExists()
        {
            var idTeste = Guid.NewGuid();
            var orderProduct = new List<OrderProduct>
            {
                new() {
                    Id = idTeste.ToString(),
                    OrderId = "ORD-123",
                    ProductId = "PROD-456",
                    Quantity = 2,
                    OrderPrice = 49.90m
                }
            };

            _elasticClientMock.Setup(x => x.GetByJsonId(It.IsAny<string>(), It.IsAny<IndexName>()))
                              .ReturnsAsync(orderProduct);

            var result = await _service.GetByIdAsync(idTeste.ToString());

            Assert.Equal(idTeste.ToString(), result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsFromRepository_WhenElasticIsEmpty()
        {
            var orderProduct = new OrderProduct
            {
                Id = Guid.NewGuid().ToString(),
                OrderId = "ORD-123",
                ProductId = "PROD-456",
                Quantity = 2,
                OrderPrice = 49.90m
            };
            _elasticClientMock.Setup(x => x.GetByJsonId(It.IsAny<string>(), It.IsAny<IndexName>()))
                              .ReturnsAsync([]);

            _orderProductRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                                 .ReturnsAsync(orderProduct);

            var result = await _service.GetByIdAsync("OP2");

            Assert.Equal(orderProduct.Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsException_AndLogsError()
        {
            _elasticClientMock.Setup(x => x.GetByJsonId(It.IsAny<string>(), It.IsAny<IndexName>()))
                              .ThrowsAsync(new Exception("fail"));

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _service.GetByIdAsync("123"));

            Assert.Contains("retrieving OrderProduct", ex.Message);
            _loggerMock.VerifyLog(LogLevel.Error, Times.Once());
        }

        [Fact]
        public async Task GetByOrderIdAsync_ReturnsFromElastic_WhenExists()
        {
            var items = new List<OrderProduct> 
            {
                new() {
                    Id = Guid.NewGuid().ToString(),
                    OrderId = "ORD-123",
                    ProductId = "PROD-456",
                    Quantity = 2,
                    OrderPrice = 49.90m
                }
            };
            _elasticClientMock.Setup(x => x.GetByJsonId(It.IsAny<string>(), It.IsAny<IndexName>()))
                              .ReturnsAsync(items);

            var result = await _service.GetByOrderIdAsync("ORDER-1");

            Assert.Equal(items.FirstOrDefault().Id, result.First().Id);
        }

        [Fact]
        public async Task GetByOrderIdAsync_ReturnsFromRepository_WhenElasticEmpty()
        {
            var items = new List<OrderProduct> 
            { 
                new() 
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderId = "ORD-123",
                    ProductId = "PROD-456",
                    Quantity = 2,
                    OrderPrice = 49.90m
                }};

            _elasticClientMock.Setup(x => x.GetByJsonId(It.IsAny<string>(), It.IsAny<IndexName>()))
                              .ReturnsAsync([]);
            _orderProductRepoMock.Setup(x => x.GetByOrderIdAsync(It.IsAny<string>()))
                                 .ReturnsAsync(items);

            var result = await _service.GetByOrderIdAsync("ORDER-2");

            Assert.Equal(items.FirstOrDefault().Id, result.First().Id);
        }

        [Fact]
        public async Task GetByOrderIdAsync_ThrowsException_AndLogsError()
        {
            _elasticClientMock.Setup(x => x.GetByJsonId(It.IsAny<string>(), It.IsAny<IndexName>()))
                              .ThrowsAsync(new Exception("fail"));

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _service.GetByOrderIdAsync("ORDER-3"));

            Assert.Contains("retrieving OrderProducts by OrderId", ex.Message);
            _loggerMock.VerifyLog(LogLevel.Error, Times.Once());
        }

        [Fact]
        public async Task GetAllAsync_ReturnsFromElastic_WhenAvailable()
        {
            var allItems = new List<OrderProduct> 
            { 
                new() 
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderId = "ORD-123",
                    ProductId = "PROD-456",
                    Quantity = 2,
                    OrderPrice = 49.90m
                } 
            };

            _elasticClientMock.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IndexName>()))
                              .ReturnsAsync(allItems);

            var result = await _service.GetAllAsync();

            Assert.Equal(allItems.FirstOrDefault().Id, result.First().Id);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsFromRepository_WhenElasticEmpty()
        {
            var allItems = new List<OrderProduct>
            { 
                new() 
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderId = "ORD-123",
                    ProductId = "PROD-456",
                    Quantity = 2,
                    OrderPrice = 49.90m
                } 
            };

            _elasticClientMock.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IndexName>()))
                              .ReturnsAsync([]);
            _orderProductRepoMock.Setup(x => x.GetAllAsync())
                                 .ReturnsAsync(allItems);

            var result = await _service.GetAllAsync();

            Assert.Equal(allItems.FirstOrDefault().Id, result.First().Id);
        }

        [Fact]
        public async Task GetAllAsync_ThrowsException_AndLogsError()
        {
            _elasticClientMock.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IndexName>()))
                              .ThrowsAsync(new Exception("fail"));

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _service.GetAllAsync());

            Assert.Contains("retrieving all OrderProducts", ex.Message);
            _loggerMock.VerifyLog(LogLevel.Error, Times.Once());
        }
    }

}

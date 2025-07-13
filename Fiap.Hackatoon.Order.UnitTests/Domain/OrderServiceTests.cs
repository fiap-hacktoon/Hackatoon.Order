using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Nodes;
using Fiap.Hackatoon.Order.Domain.Entities;
using Fiap.Hackatoon.Order.Domain.Enumerators;
using Fiap.Hackatoon.Order.Domain.Interfaces.ElasticSearch;
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
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IElasticClient<OrderEntity>> _elasticClientMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _elasticClientMock = new Mock<IElasticClient<OrderEntity>>();
            _loggerMock = new Mock<ILogger<OrderService>>();

            _orderService = new OrderService(
                _orderRepositoryMock.Object,
                _loggerMock.Object,
                _elasticClientMock.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_ReturnsFromElastic_WhenElasticHasData()
        {
            var orders = new List<OrderEntity> 
            { 
                new() { 
                    Id = "1",
                    ClientId = 123,
                    EmployeeId = 456,
                    OrderStatusId = 1
                } 
            };

            _elasticClientMock.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<int>(), "order"))
                              .ReturnsAsync(orders);

            var result = await _orderService.GetAllAsync();

            Assert.Equal(orders, result);
            _orderRepositoryMock.Verify(x => x.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsFromRepository_WhenElasticIsEmpty()
        {
            // Arrange
            var repoOrders = CreateOrderEntityList();
            _elasticClientMock.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<int>(), "order"));
            _orderRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(repoOrders);

            // Act
            var result = await _orderService.GetAllAsync();

            // Assert
            Assert.Equal(repoOrders.FirstOrDefault().Id, result.FirstOrDefault().Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsFromElastic_WhenExists()
        {
            // Arrange
            var order = CreateOrderEntityList();
            _elasticClientMock.Setup(x => x.GetByJsonId(It.IsAny<string>(), It.IsAny<IndexName>()))
                              .ReturnsAsync(order);

            // Act
            var result = await _orderService.GetByIdAsync("123");

            // Assert
            Assert.Equal(order.FirstOrDefault().Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsFromRepository_WhenNotInElastic()
        {
            // Arrange
            var order = CreateOrderEntityList().FirstOrDefault();
            _elasticClientMock.Setup(x => x.GetByJsonId(It.IsAny<string>(), It.IsAny<IndexName>()));
            _orderRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(order);

            // Act
            var result = await _orderService.GetByIdAsync("123");

            // Assert
            Assert.Equal(order, result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(99)]
        public async Task GetOrdersByStatusIdAsync_ReturnsElastic_IfAvailable(int status)
        {
            var orders = CreateOrderEntityList(status);
            _elasticClientMock.Setup(x => x.GetByStatus(status, It.IsAny<IndexName>())).ReturnsAsync(orders);

            var result = await _orderService.GetOrdersByStatusIdAsync(status);

            Assert.Equal(orders, result);
            _orderRepositoryMock.Verify(x => x.GetOrderByStatusAsync(It.IsAny<int>()), Times.Never);
        }

        [Theory]
        [InlineData(10)]
        public async Task GetOrderByClientIdAsync_FallbackToRepo_WhenElasticNull(int clientId)
        {
            var repoOrders = CreateOrderEntityList(1,
                                                   clientId = 123,
                                                   456);
            _elasticClientMock.Setup(x => x.GetByClientId(clientId, It.IsAny<IndexName>()));
            _orderRepositoryMock.Setup(x => x.GetByClientAsync(clientId)).ReturnsAsync(repoOrders);

            var result = await _orderService.GetOrderByClientIdAsync(clientId);

            Assert.Equal(repoOrders, result);
        }

        [Theory]
        [InlineData(5)]
        public async Task GetOrderByEmployeeIdAsync_ReturnsElastic_IfAvailable(int empId)
        {
            var elasticOrders = CreateOrderEntityList(1,
                                                      123,
                                                      empId);
            _elasticClientMock.Setup(x => x.GetByEmployeeId(empId, "order")).ReturnsAsync(elasticOrders);

            var result = await _orderService.GetOrderByEmployeeIdAsync(empId);

            Assert.Equal(elasticOrders, result);
        }

        [Fact]
        public async Task GetAllAsync_ThrowsAndLogs_OnError()
        {
            _elasticClientMock.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<int>(), "order"))
                              .ThrowsAsync(new Exception("Elastic down"));

            var ex = await Assert.ThrowsAsync<Exception>(() => _orderService.GetAllAsync());

            Assert.Contains("Some error occour", ex.Message);
            _loggerMock.VerifyLog(LogLevel.Error, Times.Once());
        }

        private List<OrderEntity> CreateOrderEntityList(int orderStatus = 1, 
                                                        int clientId = 123,
                                                        int employeeId = 123)
            =>
            [
                new () 
                {
                    Id = "123",
                    ClientId = clientId,
                    EmployeeId = employeeId,
                    OrderStatusId = orderStatus
                }
            ];
    }
}

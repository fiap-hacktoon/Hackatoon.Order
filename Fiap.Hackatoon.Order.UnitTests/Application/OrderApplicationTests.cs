using Fiap.Hackatoon.Order.Application.Services;
using Fiap.Hackatoon.Order.Domain.Dtos.Order;
using Fiap.Hackatoon.Order.Domain.Entities;
using Fiap.Hackatoon.Order.Domain.Enumerators;
using Fiap.Hackatoon.Order.Domain.Interfaces.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fiap.Hackatoon.Order.UnitTests.Application;

public class OrderApplicationTests
{
    private readonly Mock<IOrderService> _orderServiceMock;
    private readonly Mock<IOrderProductService> _orderProductServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IBus> _busMock;
    private readonly Mock<ILogger<OrderApplication>> _loggerMock;
    private readonly OrderApplication _orderApplication;

    public OrderApplicationTests()
    {
        _orderServiceMock = new Mock<IOrderService>();
        _orderProductServiceMock = new Mock<IOrderProductService>();
        _configurationMock = new Mock<IConfiguration>();
        _busMock = new Mock<IBus>();
        _loggerMock = new Mock<ILogger<OrderApplication>>();

        _orderApplication = new OrderApplication(
            _orderServiceMock.Object,
            _orderProductServiceMock.Object,
            _configurationMock.Object,
            _busMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetAllOrdersAsync_ReturnsOrderDtos_WhenOrdersExist()
    {
        var orderEntities = new List<OrderEntity>
        {
            new OrderEntity
            {
                Id = "1",
                ClientId = 100,
                DeliveryModeId = 2,
                OrderStatusId = 1,
                EmployeeId = 200,
                FinalPrice = 99.99m,
                Accepted = null,
                Comments = "Pedido em andamento"
            }
        };

        var orderProducts = new List<OrderProduct>
        {
            new OrderProduct
            {
                Id = "1a2b3c4d-0001-0002-0003-000000000001",
                OrderId = "1",
                ProductId = "product-abc",
                Quantity = 2,
                OrderPrice = 99.90m
            },
            new OrderProduct
            {
                Id = "1a2b3c4d-0002-0002-0003-000000000002",
                OrderId = "1",
                ProductId = "product-def",
                Quantity = 1,
                OrderPrice = 59.90m
            },
            new OrderProduct
            {
                Id = "1a2b3c4d-0003-0002-0003-000000000003",
                OrderId = "1",
                ProductId = "product-xyz",
                Quantity = 3,
                OrderPrice = 120.00m
            }
        };

        _orderServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(orderEntities);

        _orderProductServiceMock.Setup(s => s.GetByOrderIdAsync(It.IsAny<string>())).ReturnsAsync(orderProducts);

        var result = await _orderApplication.GetAllOrdersAsync();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, dto => dto.Id == "1");
    }

    [Fact]
    public async Task GetAllOrdersAsync_ReturnsEmpty_WhenNoOrders()
    {
        _orderServiceMock.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new List<OrderEntity>());

        var result = await _orderApplication.GetAllOrdersAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetOrderByIdAsync_ReturnsOrderDto_WhenOrderExists()
    {
        var order = new OrderEntity
        {
            Id = "123",
            ClientId = 101,
            OrderStatusId = 2,
            DeliveryModeId = 2,
            EmployeeId = 202,
            FinalPrice = 149.99m,
            Accepted = 1,
            Comments = "Pedido aceito"
        };

        var orderProducts = new List<OrderProduct>
        {
            new OrderProduct
            {
                Id = "1a2b3c4d-0001-0002-0003-000000000001",
                OrderId = "123",
                ProductId = "product-abc",
                Quantity = 2,
                OrderPrice = 99.90m
            }
        };

        _orderServiceMock.Setup(s => s.GetByIdAsync("123")).ReturnsAsync(order);

        _orderProductServiceMock.Setup(s => s.GetByOrderIdAsync("123"))
                                .ReturnsAsync(orderProducts);

        var result = await _orderApplication.GetOrderByIdAsync("123");

        Assert.NotNull(result);
        Assert.Equal("123", result.Id);
    }

    [Fact]
    public async Task GetOrderByIdAsync_ReturnsNull_WhenOrderDoesNotExist()
    {
        _orderServiceMock.Setup(s => s.GetByIdAsync("123"))
            .ReturnsAsync((OrderEntity)null);

        var result = await _orderApplication.GetOrderByIdAsync("123");

        Assert.Null(result);
    }

    [Fact]
    public async Task EvaluateOrder_ReturnsFailure_WhenOrderDoesNotExist()
    {
        _orderServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((OrderEntity)null);

        var result = await _orderApplication.EvaluateOrder("123", true, "Ok");

        Assert.False(result.Success);
        Assert.Contains("inexistente", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AddOrderMassTransitAsync_ReturnsFailure_WhenProductsEmpty()
    {
        var orderCreateDto = new OrderCreateDto
        {
            ClientId = 1,
            EmployeeId = 1,
            DeliveryModeId = DeliveryMode.Delivery,
            Products =
            [
                new ()
                {
                    ProductId = "1",
                    OrderPrice = 29.9m,
                    Quantity = 1
                }
            ] 
        };

        var result = await _orderApplication.AddOrderMassTransitAsync(orderCreateDto);

        Assert.False(result.Success);
        Assert.Contains("Ocorreu um problema ao tentar inserir o registro.", result.Message, StringComparison.OrdinalIgnoreCase);
    }
}

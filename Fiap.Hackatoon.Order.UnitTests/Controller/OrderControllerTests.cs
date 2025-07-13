using Fiap.Hackatoon.Order.Api.Controllers;
using Fiap.Hackatoon.Order.Domain.Dtos.Order;
using Fiap.Hackatoon.Order.Domain.Enumerators;
using Fiap.Hackatoon.Order.Domain.Interfaces.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.Hackatoon.Order.UnitTests.Controller
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderApplication> _orderAppMock;
        private readonly Mock<ILogger<OrderController>> _loggerMock;
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            _orderAppMock = new Mock<IOrderApplication>();
            _loggerMock = new Mock<ILogger<OrderController>>();
            _controller = new OrderController(_orderAppMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllOrders_ShouldReturnOrders()
        {
            // Arrange
            var orders = new List<OrderDto>
            {
                new()
                {
                    Id = "1",
                    ClientId = 123,
                    OrderStatusId = OrderStatus.Pendente,
                    EmployeeId = 123,
                    FinalPrice = 123.10m
                }
            };

            _orderAppMock.Setup(x => x.GetAllOrdersAsync()).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetAllOrders();

            // Assert
            Assert.Equal(orders, result);
        }

        [Fact]
        public async Task GetOrderById_ShouldReturnOrder_WhenExists()
        {
            var id = "1";
            var dto = new OrderDto
            {
                Id = "1",
                ClientId = 123,
                OrderStatusId = OrderStatus.Pendente,
                EmployeeId = 123,
                FinalPrice = 123.10m
            };

            _orderAppMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(dto);

            var result = await _controller.GetOrderById(id);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetOrderById_ShouldReturnNotFound_WhenNotExists()
        {
            _orderAppMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync((OrderDto)null);

            var result = await _controller.GetOrderById("nao-existe");

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetOrderByStatus_ShouldReturnList()
        {
            var dto = new OrderDto
            {
                Id = "1",
                ClientId = 123,
                OrderStatusId = OrderStatus.Pendente,
                EmployeeId = 123,
                FinalPrice = 123.10m
            };

            var orders = new List<OrderDto> { dto };
            _orderAppMock.Setup(x => x.GetOrderByStatusAsync(It.IsAny<int>())).ReturnsAsync(orders);

            var result = await _controller.GetOrderByStatus((int)OrderStatus.Pendente);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task AddOrder_ShouldReturnOk_WhenSuccess()
        {
            var dto = new OrderCreateDto
            { 
                ClientId = 123, 
                EmployeeId = 456
            };

            _orderAppMock.Setup(x => x.AddOrderMassTransitAsync(dto)).ReturnsAsync(new UpsertOrderResponse { Success = true, Message = "Pedido criado" });

            var result = await _controller.AddOrder(dto);
            Assert.NotNull(result);
            Assert.IsType<ActionResult<UpsertOrderResponse>>(result);
        }

        [Fact]
        public async Task AddOrder_ShouldReturnBadRequest_WhenFails()
        {
            var dto = new OrderCreateDto
            {
                ClientId = 123,
                EmployeeId = 456
            };

            _orderAppMock.Setup(x => x.AddOrderMassTransitAsync(dto)).ReturnsAsync(new UpsertOrderResponse { Success = false, Message = "Erro" });

            var result = await _controller.AddOrder(dto);

            Assert.IsType<ActionResult<UpsertOrderResponse>>(result);
        }

        [Fact]
        public async Task EvaluateOrder_ShouldReturnOk_WhenSuccess()
        {
            _orderAppMock.Setup(x => x.EvaluateOrder(It.IsAny<string>(), true, "ok")).ReturnsAsync(new UpsertOrderResponse { Success = true, Message = "Avaliado" });

            var result = await _controller.EvaluateOrder("1", true, "ok");

            Assert.NotNull(result);
            Assert.IsType<ActionResult<UpsertOrderResponse>>(result);
        }

        [Fact]
        public async Task UpdateOrder_ShouldReturnBadRequest_WhenFails()
        {
            var dto = new OrderDto
            { 
                Id = "123",
                ClientId = 123,
                EmployeeId = 456,
                OrderStatusId = OrderStatus.EmPreparacao,
                FinalPrice = 100.09m
            };
            _orderAppMock.Setup(x => x.UpdateOrderMassTransitAsync(dto)).ReturnsAsync(new UpsertOrderResponse { Success = false, Message = "Falha" });

            var result = await _controller.UpdateOrder(dto);

            Assert.IsType<ActionResult<UpsertOrderResponse>>(result);
        }

        [Fact]
        public async Task DeleteOrder_ShouldReturnOk_WhenSuccess()
        {
            _orderAppMock.Setup(x => x.DeleteOrderMassTransitAsync(It.IsAny<string>())).ReturnsAsync(new UpsertOrderResponse { Success = true, Message = "Deletado" });

            var result = await _controller.DeleteOrder("123");

            Assert.NotNull(result);
            Assert.IsType<ActionResult<UpsertOrderResponse>>(result);
        }
    }
}

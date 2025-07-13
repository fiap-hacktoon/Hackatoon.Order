using Fiap.Hackatoon.Order.Domain.Dtos.Order;
using Fiap.Hackatoon.Order.Domain.Interfaces.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Hackatoon.Order.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(IOrderApplication orderApplication, ILogger<OrderController> logger) : ControllerBase
    {        
        private readonly IOrderApplication _orderService = orderApplication;
        private readonly ILogger<OrderController> _logger = logger;

        [HttpGet]
        [Authorize(Roles = "Manager,Attendant,Kitchen,Client")]
        public async Task<IEnumerable<OrderDto>> GetAllOrders()
        {
            _logger.LogInformation("Buscando todos os pedidos");
            return await _orderService.GetAllOrdersAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Manager,Attendant,Kitchen,Client")]
        public async Task<ActionResult<OrderDto>> GetOrderById(string id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning($"Pedido de ID {id} não encontrado");
                return NotFound();
            }

            return Ok(order);
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Manager,Attendant,Kitchen")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrderByStatus(int status)
        {
            _logger.LogInformation("Buscando pedidos pelo Status {Status}", status);

            var orders = await _orderService.GetOrderByStatusAsync(status);
            return Ok(orders);
        }

        [HttpGet("employee/{id}")]
        [Authorize(Roles = "Manager,Attendant,Kitchen")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrderByEmployeeId(int id)
        {
            _logger.LogInformation("Buscando pedidos pelo Id de Funcionario {Id}", id);
            var orders = await _orderService.GetOrderByEmployeeIdAsync(id);
            return Ok(orders);
        }

        [HttpGet("client/{id}")]
        [Authorize(Roles = "Manager,Attendant,Kitchen")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrderByClientId(int id)
        {
            _logger.LogInformation("Buscando pedidos pelo Id de Cliente {Id}", id);
            var orders = await _orderService.GetOrderByClientIdAsync(id);
            return Ok(orders);
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Attendant,Kitchen,Client")]
        public async Task<ActionResult<UpsertOrderResponse>> AddOrder([FromBody] OrderCreateDto dto)
        {
            _logger.LogInformation("Adicionando novo pedido");
            var insertedObject = await _orderService.AddOrderMassTransitAsync(dto);
            if (insertedObject.Success)
                return Ok(insertedObject.Message);
            else
                return BadRequest(insertedObject.Message);
        }

        [HttpPut("evaluate/{id}")]
        [Authorize(Roles = "Manager,Kitchen")]
        public async Task<ActionResult<UpsertOrderResponse>> EvaluateOrder(string id, [FromQuery] bool accepted, [FromQuery] string comments)
        {
            _logger.LogInformation("Aprovando pedido...");
            var result = await _orderService.EvaluateOrder(id, accepted, comments);
            if (result.Success)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }

        [HttpPut]
        [Authorize(Roles = "Manager,Attendant,Kitchen,Client")]
        public async Task<ActionResult<UpsertOrderResponse>> UpdateOrder([FromBody] OrderDto dto)
        {
            _logger.LogInformation("Atualizando pedido");
            var result = await _orderService.UpdateOrderMassTransitAsync(dto);
            if (result.Success)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<UpsertOrderResponse>> DeleteOrder(string id)
        {
            _logger.LogInformation("Excluindo pedido");
            var result = await _orderService.DeleteOrderMassTransitAsync(id);
            if (result.Success)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }
    }
}

using Fiap.Hackatoon.Order.Domain.Dtos.Order;
using Fiap.Hackatoon.Order.Domain.Enumerators;
using Fiap.Hackatoon.Order.Domain.Extensions;
using Fiap.Hackatoon.Order.Domain.Interfaces.Application;
using Fiap.Hackatoon.Order.Domain.Interfaces.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Fiap.Hackatoon.Order.Application.Services
{
    public class OrderApplication(IOrderService orderService,
                                  IOrderProductService orderProductService,
                                  IConfiguration configuration,
                                  IBus bus,
                                  ILogger<OrderApplication> logger) : IOrderApplication
    {
        private readonly IOrderService _orderService = orderService;

        private readonly IConfiguration _configuration = configuration;

        private readonly IOrderProductService _orderProductService = orderProductService;

        private readonly ILogger<OrderApplication> _logger = logger;

        private readonly IBus _bus = bus;

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _orderService.GetAllAsync();
                if (orders == null || !orders.Any())
                    return [];

                // Executar em paralelo
                var tasks = orders.Select(async order =>
                {
                    var orderProducts = await _orderProductService.GetByOrderIdAsync(order.Id);
                    return order.ToOrderDto(orderProducts);
                });

                var orderDtos = await Task.WhenAll(tasks);
                return orderDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro na consulta de todos os pedidos.");
                return [];
            }
        }

        public async Task<OrderDto?> GetOrderByIdAsync(string id)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(id);
                if (order == null)
                    return null;

                var orderProducts = await _orderProductService.GetByOrderIdAsync(order.Id);
                return order.ToOrderDto(orderProducts);
            }
            catch (Exception e)
            {
                _logger.LogError($"Ocorreu um erro na consulta de um pedido por Id. Erro: {e.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<OrderDto>> GetOrderByStatusAsync(int status)
        {
            try
            {
                var orders = await _orderService.GetOrdersByStatusIdAsync(status);
                if (orders == null)
                    return null;

                var tasks = orders.Select(async order =>
                {
                    var orderProducts = await _orderProductService.GetByOrderIdAsync(order.Id);
                    return order.ToOrderDto(orderProducts);
                });

                var orderDtos = await Task.WhenAll(tasks);
                return orderDtos;
            }
            catch (Exception e)
            {
                _logger.LogError($"Ocorreu um erro na consulta dos pedidos pela situação. Erro: {e.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<OrderDto>> GetOrderByEmployeeIdAsync(int id)
        {
            try
            {
                var orders = await _orderService.GetOrderByEmployeeIdAsync(id);
                if (orders == null)
                    return null;

                var tasks = orders.Select(async order =>
                {
                    var orderProducts = await _orderProductService.GetByOrderIdAsync(order.Id);
                    return order.ToOrderDto(orderProducts);
                });

                var orderDtos = await Task.WhenAll(tasks);
                return orderDtos;
            }
            catch (Exception e)
            {
                _logger.LogError($"Ocorreu um erro na consulta dos pedidos realizados pelo funcionario. Erro: {e.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<OrderDto>> GetOrderByClientIdAsync(int id)
        {
            try
            {
                var orders = await _orderService.GetOrderByClientIdAsync(id);
                if (orders == null)
                    return null;

                var tasks = orders.Select(async order =>
                {
                    var orderProducts = await _orderProductService.GetByOrderIdAsync(order.Id);
                    return order.ToOrderDto(orderProducts);
                });

                var orderDtos = await Task.WhenAll(tasks);
                return orderDtos;
            }
            catch (Exception e)
            {
                _logger.LogError($"Ocorreu um erro na consulta dos pedidos realizados pelo cliente. Erro: {e.Message}");
                return null;
            }
        }

        public async Task<UpsertOrderResponse> EvaluateOrder(string id, bool accepted, string comments)
        {
            var insertResult = new UpsertOrderResponse();
            try
            {
                var orderExist = await GetOrderByIdAsync(id);
                if (orderExist == null)
                {
                    insertResult.Success = false;
                    insertResult.Message = $"Pedido inexistente na base de dados para atualizacao.";
                }
                else
                {
                    var evaluatedOrderDto = orderExist.ToOrderEvalueatedDto(accepted, comments);
                    await UpdateOrderMassTransitAsync(evaluatedOrderDto);
                }
            }
            catch (Exception e)
            {
                insertResult.Success = false;
                insertResult.Message = $"Ocorreu um problema ao tentar avaliar um pedido.";
                _logger.LogError(insertResult.Message, e);
            }

            return insertResult;
        }

        public async Task<UpsertOrderResponse> AddOrderMassTransitAsync(OrderCreateDto orderCreateDto)
        {
            var insertResult = new UpsertOrderResponse();

            if (orderCreateDto.Products.IsNullOrEmpty())
            {
                insertResult.Success = false;
                insertResult.Message = $"Lista de produtos precisa ter pelo menos um produto no pedido.";
                _logger.LogWarning(insertResult.Message);
                return insertResult;
            }

            foreach (var product in orderCreateDto.Products)
            {
                var validatedProduct = await _orderProductService.ValidateProduct(product.ProductId.ToString());
                if (validatedProduct is null)
                {
                    insertResult.Success = false;
                    insertResult.Message = $"Produto de Id {product.ProductId} não existente no banco de dados.";
                    _logger.LogWarning(insertResult.Message);
                    break;
                }
            }

            try
            {
                var massTransitObject = _configuration.ChargeMassTransitObject();

                var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{massTransitObject.QueueList.InsertQueue}"));

                await endpoint.Send<OrderCreateDto>(orderCreateDto);

                insertResult.Success = true;
                insertResult.Message = "Pedido inserido na FILA com sucesso.";
            }
            catch (Exception e)
            {
                insertResult.Success = false;
                insertResult.Message = $"Ocorreu um problema ao tentar inserir o registro.";
                _logger.LogError(insertResult.Message, e);
            }

            return insertResult;
        }

        public async Task<UpsertOrderResponse> UpdateOrderMassTransitAsync(OrderDto orderUpdateDto)
        {
            var updateResult = new UpsertOrderResponse();

            if (!orderUpdateDto.Evaluation)
            {
                var orderExist = await GetOrderByIdAsync(orderUpdateDto.Id);
                if (orderExist == null)
                {
                    updateResult.Success = false;
                    updateResult.Message = $"Pedido inexistente na base de dados para atualizacao.";

                    return updateResult;
                }

                if (orderUpdateDto.OrderStatusId.IsInvalidStatusUpdate())
                {
                    updateResult.Success = false;
                    updateResult.Message = $"Pedido ja se encontra em uma situação impossivel de se alterar.";

                    return updateResult;
                }

                else
                {
                    if (orderUpdateDto.Products.IsNullOrEmpty())
                    {
                        updateResult.Success = false;
                        updateResult.Message = $"Lista de produtos precisa ter pelo menos um produto no pedido.";
                        _logger.LogWarning(updateResult.Message);
                        return updateResult;
                    }

                    foreach (var product in orderUpdateDto.Products)
                    {
                        var validatedProduct = _orderProductService.ValidateProduct(product.Id.ToString());
                        if (validatedProduct is null)
                        {
                            updateResult.Success = false;
                            updateResult.Message = $"Produto de Id {product.Id} não existente no banco de dados.";
                            _logger.LogWarning(updateResult.Message);
                            break;
                        }
                    }
                }
            }

            try
            {
                var massTransitObject = _configuration.ChargeMassTransitObject();

                var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{massTransitObject.QueueList.UpdateQueue}"));

                await endpoint.Send<Domain.Dtos.Order.OrderDto>(orderUpdateDto);

                updateResult.Success = true;
                updateResult.Message = "Pedido inserido na FILA para atualizacao com sucesso.";
            }
            catch (Exception e)
            {
                updateResult.Success = false;
                updateResult.Message = $"Ocorreu um problema ao tentar inserir o pedido na fila para atualizacao.";
                _logger.LogError(updateResult.Message, e);
            }

            return updateResult;
        }

        public async Task<UpsertOrderResponse> DeleteOrderMassTransitAsync(string id)
        {
            var insertResult = new UpsertOrderResponse();

            var orderExist = await GetOrderByIdAsync(id);

            if (orderExist == null)
            {
                insertResult.Success = false;
                insertResult.Message = $"Contato nao encontrado na base para delecao.";
            }
            else
            {
                try
                {
                    var massTransitObject = _configuration.ChargeMassTransitObject();

                    var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{massTransitObject.QueueList.DeleteQueue}"));

                    await endpoint.Send<Domain.Dtos.Order.OrderDto>(orderExist);

                    insertResult.Success = true;
                    insertResult.Message = "Contato inserido na FILA para delecao com sucesso.";
                }
                catch (Exception e)
                {
                    insertResult.Success = false;
                    insertResult.Message = $"Ocorreu um problema ao tentar inserir o registro na fila para delecao.";
                    _logger.LogError(insertResult.Message, e);
                }
            }

            return insertResult;
        }

        public async Task AddOrderAsync(OrderCreateDto orderCreateDto)
        {
            try
            {
                await _orderService.InsertAsync(orderCreateDto.ToAddEntity());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception(e.Message);
            }
        }

        public async Task DeleteOrderAsync(OrderDto orderDto)
        {
            try
            {
                await _orderService.DeleteAsync(orderDto.ToEntity());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception(e.Message);
            }
        }

        public async Task UpdateOrderAsync(OrderDto orderDto)
        {
            try
            {
                await _orderService.UpdateAsync(orderDto.ToEntity());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception(e.Message);
            }
        }
    }
}

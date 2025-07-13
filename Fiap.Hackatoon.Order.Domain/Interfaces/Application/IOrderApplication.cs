using Fiap.Hackatoon.Order.Domain.Dtos.Order;

namespace Fiap.Hackatoon.Order.Domain.Interfaces.Application
{
    public interface IOrderApplication
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();

        Task<OrderDto?> GetOrderByIdAsync(string id);

        Task<IEnumerable<OrderDto>> GetOrderByStatusAsync(int status);

        Task<IEnumerable<OrderDto>> GetOrderByEmployeeIdAsync(int id);

        Task<IEnumerable<OrderDto>> GetOrderByClientIdAsync(int id);

        Task<UpsertOrderResponse> AddOrderMassTransitAsync(OrderCreateDto orderCreateDto);

        Task<UpsertOrderResponse> EvaluateOrder(string id, bool accepted, string comments);

        Task<UpsertOrderResponse> UpdateOrderMassTransitAsync(OrderDto orderUpdateDto);

        Task<UpsertOrderResponse> DeleteOrderMassTransitAsync(string id);

        Task AddOrderAsync(OrderCreateDto orderCreateDto);

        Task DeleteOrderAsync(OrderDto orderDto);

        Task UpdateOrderAsync(OrderDto orderDto);
    }
}

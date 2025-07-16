using Fiap.Hackatoon.Order.Domain.Dtos.Order;
using Fiap.Hackatoon.Order.Domain.Enumerators;

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

        Task<UpsertOrderResponse> UpdateOrderMassTransitAsync(OrderUpdateDto orderUpdateDto, bool evaluation = false);

        Task<UpsertOrderResponse> DeleteOrderMassTransitAsync(string id);

        Task<UpsertOrderResponse> ChangeOrderStatus(string id, OrderStatus orderStatus);
    }
}

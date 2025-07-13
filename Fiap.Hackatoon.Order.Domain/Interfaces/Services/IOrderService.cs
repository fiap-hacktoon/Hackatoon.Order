using Fiap.Hackatoon.Order.Domain.Entities;

namespace Fiap.Hackatoon.Order.Domain.Interfaces.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderEntity>> GetAllAsync();

        Task<OrderEntity> GetByIdAsync(string id);

        Task<IEnumerable<OrderEntity>> GetOrdersByStatusIdAsync(int status);

        Task<IEnumerable<OrderEntity>> GetOrderByClientIdAsync(int clientId);

        Task<IEnumerable<OrderEntity>> GetOrderByEmployeeIdAsync(int employeeId);

        Task InsertAsync(OrderEntity order);

        Task DeleteAsync(OrderEntity order);

        Task UpdateAsync(OrderEntity order);
    }
}
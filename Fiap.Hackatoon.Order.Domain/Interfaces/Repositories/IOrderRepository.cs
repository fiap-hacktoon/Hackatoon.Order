using Fiap.Hackatoon.Order.Domain.Entities;

namespace Fiap.Hackatoon.Order.Domain.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(OrderEntity contact); 

        Task UpdateAsync(OrderEntity contact);

        Task<OrderEntity> GetByIdAsync(string orderId);

        Task<IEnumerable<OrderEntity>> GetAllAsync();

        Task<IEnumerable<OrderEntity>> GetByClientAsync(int clientId);

        Task<IEnumerable<OrderEntity>> GetByEmployeeAsync(int employeeId);

        Task<IEnumerable<OrderEntity>> GetOrderByStatusAsync(int status);

        Task DeleteAsync(OrderEntity order);
    }
}

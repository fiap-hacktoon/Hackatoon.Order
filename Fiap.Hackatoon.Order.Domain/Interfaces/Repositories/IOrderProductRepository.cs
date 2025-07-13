using Fiap.Hackatoon.Order.Domain.Dtos.Order;
using Fiap.Hackatoon.Order.Domain.Entities;

namespace Fiap.Hackatoon.Order.Domain.Interfaces.Repositories
{
    public interface IOrderProductRepository
    {
        Task<OrderProduct> GetByIdAsync(string id);

        Task<List<OrderProduct?>> GetByOrderIdAsync(string orderId);

        Task<List<OrderProduct>> GetAllAsync();

        Task AddAsync(OrderProduct orderProduct);

        Task UpdateAsync(OrderProduct orderProduct);

        Task DeleteAsync(OrderProduct orderProduct);
    }
}
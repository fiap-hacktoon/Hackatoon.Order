using Fiap.Hackatoon.Order.Domain.Dtos.Order;
using Fiap.Hackatoon.Order.Domain.Entities;

namespace Fiap.Hackatoon.Order.Domain.Interfaces.Services
{
    public interface IOrderProductService
    {
        Task<Product> ValidateProduct(string id);

        Task<OrderProduct> GetByIdAsync(string id);

        Task<List<OrderProduct?>> GetByOrderIdAsync(string orderId);

        Task<List<OrderProduct>> GetAllAsync();

        Task AddAsync(OrderProduct orderProduct);

        Task UpdateAsync(OrderProduct orderProduct);

        Task DeleteAsync(OrderProduct orderProduct);
    }
}
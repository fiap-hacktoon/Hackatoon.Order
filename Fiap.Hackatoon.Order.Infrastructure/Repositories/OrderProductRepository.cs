using Fiap.Hackatoon.Order.Domain.Dtos.Order;
using Fiap.Hackatoon.Order.Domain.Entities;
using Fiap.Hackatoon.Order.Domain.Interfaces.Repositories;
using Fiap.Hackatoon.Order.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Hackatoon.Order.Infrastructure.Repositories
{
    public class OrderProductRepository(OrderDbContext context) : IOrderProductRepository
    {
        private readonly OrderDbContext _context = context;

        public async Task<OrderProduct> GetByIdAsync(string id)
            => await _context.OrderProduct.FindAsync(id);

        public async Task<List<OrderProduct?>> GetByOrderIdAsync(string orderId)
            => await _context.OrderProduct.Where(o => o.OrderId == orderId).ToListAsync();

        public async Task<List<OrderProduct>> GetAllAsync()
            => await _context.OrderProduct.ToListAsync();

        public async Task AddAsync(OrderProduct orderProduct)
        {
            await _context.OrderProduct.AddAsync(orderProduct);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrderProduct orderProduct)
        {
            _context.OrderProduct.Update(orderProduct);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(OrderProduct orderProduct)
        {
            _context.OrderProduct.Remove(orderProduct);
            await _context.SaveChangesAsync();
        }
    }
}

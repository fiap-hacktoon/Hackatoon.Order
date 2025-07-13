using Fiap.Hackatoon.Order.Domain.Entities;
using Fiap.Hackatoon.Order.Domain.Interfaces.Repositories;
using Fiap.Hackatoon.Order.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Hackatoon.Order.Infrastructure.Repositories
{
    public class OrderRepository(OrderDbContext context) : IOrderRepository
    {
        private readonly OrderDbContext _context = context;

        public async Task<OrderEntity> GetByIdAsync(string id)
            => await _context.OrderEntity.FindAsync(id);

        public async Task<OrderEntity?> GetByClientIdAsync(string orderId)
            => await _context.OrderEntity.FirstOrDefaultAsync(o => o.Id == orderId);
        
        public async Task<IEnumerable<OrderEntity>> GetAllAsync()
            => await _context.OrderEntity.ToListAsync();

        public async Task<IEnumerable<OrderEntity>> GetByClientAsync(int clientId)
            => await _context.OrderEntity.Where(c => c.ClientId == clientId).ToListAsync();

        public async Task<IEnumerable<OrderEntity>> GetByEmployeeAsync(int employeeId)
            => await _context.OrderEntity.Where(c => c.EmployeeId == employeeId).ToListAsync();

        public async Task<IEnumerable<OrderEntity>> GetOrderByStatusAsync(int status)
            => await _context.OrderEntity.Where(c => c.OrderStatusId == status).ToListAsync();
        
        public async Task AddAsync(OrderEntity order)
        {
            await _context.OrderEntity.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrderEntity order)
        {
            _context.OrderEntity.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(OrderEntity order)
        {
            _context.OrderEntity.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}
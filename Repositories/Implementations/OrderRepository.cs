using Microsoft.EntityFrameworkCore;
using ShopNext.Constants;
using ShopNext.Data;
using ShopNext.Models;
using ShopNext.Repositories.Interfaces;

namespace ShopNext.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ShopNextDbContext _context;

        public OrderRepository(ShopNextDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetByIdAsync(int id, int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
        }

        public async Task<Order?> GetByIdForUpdateAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetUserOrdersAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payment)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> UpdateStatusAsync(int id, string status)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return null;

            order.Status = status;
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<bool> HasUserPurchasedProductAsync(int userId, int productId)
        {
            return await _context.Orders
                .AnyAsync(o => o.UserId == userId
                    && o.Status == OrderStatuses.Delivered
                    && o.PaymentStatus == PaymentStatuses.Paid
                    && o.OrderItems.Any(oi => oi.ProductId == productId));
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
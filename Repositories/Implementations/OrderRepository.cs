using ShopNext.Data;
using ShopNext.Models;
using ShopNext.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return null;

            order.Status = status;
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> UpdatePaymentStatusAsync(int id, string paymentStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return null;

            order.PaymentStatus = paymentStatus;
            await _context.SaveChangesAsync();
            return order;
        }
        public async Task UpdateRazorpayOrderIdAsync(int orderId, string razorpayOrderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return;

            order.RazorpayOrderId = razorpayOrderId;
            await _context.SaveChangesAsync();
        }

        public async Task<Payment?> GetPaymentByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == orderId);
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }
    }
}

using ShopNext.Models;

namespace ShopNext.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order);
        Task<Order?> GetByIdAsync(int id, int userId);
        Task<List<Order>> GetUserOrdersAsync(int userId);
        Task<Order?> UpdateStatusAsync(int id, string status);
        Task<Order?> UpdatePaymentStatusAsync(int id, string paymentStatus);
    }
}

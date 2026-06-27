using ShopNext.Models;

namespace ShopNext.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order);

        Task<Order?> GetByIdAsync(int id, int userId);
        Task<Order?> GetByIdForUpdateAsync(int id);

        Task<List<Order>> GetUserOrdersAsync(int userId);

        Task<Order?> UpdateStatusAsync(int id, string status);

        Task<bool> HasUserPurchasedProductAsync(int userId, int productId);
        Task SaveChangesAsync();
    }
}
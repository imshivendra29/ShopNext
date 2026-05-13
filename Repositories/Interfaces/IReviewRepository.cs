using ShopNext.Models;
namespace ShopNext.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetByProductIdAsync(int productId);
        Task<Review> CreateAsync(Review review);
        Task<bool> DeleteAsync(int id, int userId);
        Task<Review?> GetByUserAndProductAsync(int userId, int productId);
        Task<Review?> UpdateAsync(int id, int userId, int rating, string? comment);
    }
}

using ShopNext.Models;

namespace ShopNext.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateAsync(Product product);
        Task<Product?> UpdateAsync(int id, Product product);
        Task<bool> DeleteAsync(int id);
        Task<(List<Product> Products, int TotalCount)> SearchAsync(
    string? keyword,
    int? categoryId,
    decimal? minPrice,
    decimal? maxPrice,
    string? sortBy,
    int page,
    int pageSize);
    }
}

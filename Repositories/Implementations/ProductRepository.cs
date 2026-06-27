using ShopNext.Data;
using ShopNext.Models;
using ShopNext.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace ShopNext.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShopNextDbContext _context;

        public ProductRepository(ShopNextDbContext context)
        {
            _context = context;
        }
        public async Task<(List<Product> Products, int TotalCount)> SearchAsync(
           string? keyword,
           int? categoryId,
           decimal? minPrice,
            decimal? maxPrice,
           string? sortBy,
           int page,
           int pageSize)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(p => p.Name.Contains(keyword) ||
                                         p.Description.Contains(keyword));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "rating" => query.OrderByDescending(p => p.AverageRating),
                "reviews" => query.OrderByDescending(p => p.ReviewCount),
                "newest" => query.OrderByDescending(p => p.DateCreated),
                _ => query.OrderByDescending(p => p.DateCreated)
            };


            var totalCount = await query.CountAsync();


            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }
        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }
        public async Task<List<Product>> GetProductsByIdsAsync(List<int> productIds)
        {
            return await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            await _context.Entry(product)
                .Reference(p => p.Category)
                .LoadAsync();

            return product;
        }
        //fix
        public async Task<Dictionary<int, decimal>> GetFreshPricesAsync(List<int> productIds)
        {
            return await _context.Products
                .Where(p => productIds.Contains(p.Id) && p.IsActive)
                .ToDictionaryAsync(p => p.Id, p => p.Price);
        }
        public async Task RecalculateRatingAsync(int productId)
        {
            var stats = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .GroupBy(r => r.ProductId)
                .Select(g => new
                {
                    Avg = g.Average(r => (double)r.Rating),
                    Count = g.Count()
                })
                .FirstOrDefaultAsync();

            await _context.Products
                .Where(p => p.Id == productId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.AverageRating, stats == null ? 0 : stats.Avg)
                    .SetProperty(p => p.ReviewCount, stats == null ? 0 : stats.Count));
        }
        public async Task<bool> DeductStockAsync(int productId, int quantity)
        {
            
            var updated = await _context.Products
                .Where(p => p.Id == productId && p.Stock >= quantity)
                .ExecuteUpdateAsync(s => s.SetProperty(
                    p => p.Stock, p => p.Stock - quantity));

            return updated > 0; 
        }
        public async Task<Product?> UpdateAsync(int id, Product product)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null) return null;

            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.Stock = product.Stock;
            existing.ImageUrl = product.ImageUrl;
            existing.CategoryId = product.CategoryId;
            existing.IsActive = product.IsActive;
            existing.IsCodAvailable = product.IsCodAvailable;
            await _context.SaveChangesAsync();
            return existing;
        }
     
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null) return false;
            _context.Products.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

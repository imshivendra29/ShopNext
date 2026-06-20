using ShopNext.Data;
using ShopNext.Models;
using ShopNext.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace ShopNext.Repositories.Implementations
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ShopNextDbContext _context;

        public ReviewRepository(ShopNextDbContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync();
        }

        public async Task<Review> CreateAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            await _context.Entry(review)
                .Reference(r => r.User)
                .LoadAsync();

            return review;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (review == null) return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Review?> GetByUserAndProductAsync(int userId, int productId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);
        }

        public async Task<Review?> UpdateAsync(int id, int userId, int rating, string? comment)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (review == null) return null;

            review.Rating = rating;
            if (comment != null)
                review.Comment = comment;

            await _context.SaveChangesAsync();
            return review;
        }
        public async Task<Review?> GetByIdAsync(int id, int userId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        }
    }
}

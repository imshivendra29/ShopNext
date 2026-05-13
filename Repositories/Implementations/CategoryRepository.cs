using ShopNext.Data;
using ShopNext.Models;
using ShopNext.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace ShopNext.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ShopNextDbContext _context;

        public CategoryRepository(ShopNextDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Category> CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> UpdateAsync(int id, Category category)
        {
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return null;

            existing.Name = category.Name;
            existing.Description = category.Description;
            existing.ImageUrl = category.ImageUrl;
            existing.IsActive = category.IsActive;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return false;

            _context.Categories.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

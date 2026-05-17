using Microsoft.EntityFrameworkCore;
using ShopNext.Data;
using ShopNext.Models;
using ShopNext.Repositories.Interfaces;

namespace ShopNext.Repositories.Implementations
{
    public class BannerRepository : IBannerRepository
    {
        private readonly ShopNextDbContext _context;

        public BannerRepository(ShopNextDbContext context)
        {
            _context = context;
        }

        public async Task<List<Banner>> GetAllAsync()
        {
            return await _context.Banners
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<Banner> AddAsync(Banner banner)
        {
            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();
            return banner;
        }

        public async Task DeleteAsync(int id)
        {
            var banner = await _context.Banners.FindAsync(id);

            if (banner == null)
                throw new Exception("Banner not found");

            banner.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}
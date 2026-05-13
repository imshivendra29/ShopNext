using Microsoft.EntityFrameworkCore;
using ShopNext.Models;

namespace ShopNext.Data
{
    public class ShopNextDbContext : DbContext
    {
        public ShopNextDbContext(DbContextOptions<ShopNextDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
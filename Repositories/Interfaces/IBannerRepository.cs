using ShopNext.Models;
namespace ShopNext.Repositories.Interfaces
{
    public interface IBannerRepository
    {
        Task<List<Banner>> GetAllAsync();
        Task<Banner> AddAsync(Banner banner);
        Task DeleteAsync(int id);
    }
}

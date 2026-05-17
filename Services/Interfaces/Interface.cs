using Microsoft.AspNetCore.Http;
using ShopNext.Models;

namespace ShopNext.Services.Interfaces
{
    public interface IBannerService
    {
        Task<List<Banner>> GetAllAsync();
        Task<Banner> CreateAsync(IFormFile image);
        Task DeleteAsync(int id);
    }
}
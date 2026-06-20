using Microsoft.AspNetCore.Http;
using ShopNext.DTOs.Banners;
using ShopNext.Models;

namespace ShopNext.Services.Interfaces
{
    public interface IBannerService
    {
        Task<List<BannerResponseDto>> GetAllAsync();
        Task<BannerResponseDto> CreateAsync(IFormFile image);
        Task DeleteAsync(int id);
    }
}
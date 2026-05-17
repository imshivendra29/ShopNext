using Microsoft.AspNetCore.Http;
using ShopNext.Models;
using ShopNext.Repositories.Interfaces;
using ShopNext.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace ShopNext.Services.Implementations
{
    public class BannerService : IBannerService
    {
        private readonly IBannerRepository _bannerRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public BannerService(
            IBannerRepository bannerRepository,
            ICloudinaryService cloudinaryService)
        {
            _bannerRepository = bannerRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<List<Banner>> GetAllAsync()
        {
            return await _bannerRepository.GetAllAsync();
        }

        public async Task<Banner> CreateAsync(IFormFile image)
        {
            var imageUrl = await _cloudinaryService.UploadImageAsync(image, "banners");

            var banner = new Banner
            {
                ImageUrl = imageUrl,
                IsActive = true
            };

            return await _bannerRepository.AddAsync(banner);
        }
        public async Task DeleteAsync(int id)
        {
            await _bannerRepository.DeleteAsync(id);
        }
    }
}
using Microsoft.AspNetCore.Http;
using ShopNext.DTOs.Banners;
using ShopNext.Models;
using ShopNext.Infrastructure.Cloudinary.Interfaces;
using ShopNext.Infrastructure.Redis;

using ShopNext.Repositories.Interfaces;
using ShopNext.Services.Interfaces;

namespace ShopNext.Services.Implementations
{
    public class BannerService : IBannerService
    {
        private readonly IBannerRepository _bannerRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IRedisCacheService _cache;

        private static readonly SemaphoreSlim _cacheLock = new(1, 1);

        public BannerService(
            IBannerRepository bannerRepository,
            ICloudinaryService cloudinaryService,
            IRedisCacheService cache)
        {
            _bannerRepository = bannerRepository;
            _cloudinaryService = cloudinaryService;
            _cache = cache;
        }

        public async Task<List<BannerResponseDto>> GetAllAsync()
        {
            var cached = await _cache.GetAsync<List<BannerResponseDto>>(RedisKeys.Banners);

            if (cached is not null)
                return cached;

            await _cacheLock.WaitAsync();

            try
            {
                cached = await _cache.GetAsync<List<BannerResponseDto>>(RedisKeys.Banners);

                if (cached is not null)
                    return cached;

                var banners = await _bannerRepository.GetAllAsync();

                var response = banners
                    .Where(x => x.IsActive)
                    .Select(x => new BannerResponseDto
                    {
                        Id = x.Id,
                        ImageUrl = x.ImageUrl,
                        IsActive = x.IsActive
                    })
                    .ToList();

                await _cache.SetAsync(
                    RedisKeys.Banners,
                    response,
                    TimeSpan.FromMinutes(10));

                return response;
            }
            finally
            {
                _cacheLock.Release();
            }
        }

        public async Task<BannerResponseDto> CreateAsync(IFormFile image)
        {
            var imageUrl = await _cloudinaryService.UploadImageAsync(image, "banners");

            var banner = new Banner
            {
                ImageUrl = imageUrl,
                IsActive = true
            };

            var result = await _bannerRepository.AddAsync(banner);

            await _cache.DeleteAsync(RedisKeys.Banners);

            return new BannerResponseDto
            {
                Id = result.Id,
                ImageUrl = result.ImageUrl,
                IsActive = result.IsActive
            };
        }

        public async Task DeleteAsync(int id)
        {
            await _bannerRepository.DeleteAsync(id);

            await _cache.DeleteAsync(RedisKeys.Banners);
        }
    }
}
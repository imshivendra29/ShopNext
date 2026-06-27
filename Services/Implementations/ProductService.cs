using ShopNext.DTOs.Product;
using ShopNext.Infrastructure.Cloudinary.Interfaces;
using ShopNext.Infrastructure.Redis;
using ShopNext.Models;
using ShopNext.Repositories.Interfaces;

namespace ShopNext.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IRedisCacheService _cache;

        public ProductService(
            IProductRepository repository,
            ICloudinaryService cloudinaryService,
            IRedisCacheService cache)
        {
            _repository = repository;
            _cloudinaryService = cloudinaryService;
            _cache = cache;
        }

        public async Task<ProductSearchResponseDto> SearchAsync(ProductSearchDto dto)
        {
            if (dto.PageSize > 50) dto.PageSize = 50;

            var cacheKey = RedisKeys.ProductSearch(
                dto.Keyword,
                dto.CategoryId,
                dto.MinPrice,
                dto.MaxPrice,
                dto.SortBy,
                dto.Page,
                dto.PageSize);

            var cached = await _cache.GetAsync<ProductSearchResponseDto>(cacheKey);
            if (cached is not null) return cached;

            var (products, totalCount) = await _repository.SearchAsync(
                dto.Keyword,
                dto.CategoryId,
                dto.MinPrice,
                dto.MaxPrice,
                dto.SortBy,
                dto.Page,
                dto.PageSize);

            var result = new ProductSearchResponseDto
            {
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / dto.PageSize),
                CurrentPage = dto.Page,
                PageSize = dto.PageSize,
                Products = products.Select(MapToResponse).ToList()
            };

            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(2));
            return result;
        }

        public async Task<List<ProductResponseDto>> GetAllAsync()
        {
            var cached = await _cache.GetAsync<List<ProductResponseDto>>(RedisKeys.AllProducts);
            if (cached is not null) return cached;

            var products = await _repository.GetAllAsync();
            var result = products.Select(MapToResponse).ToList();

            await _cache.SetAsync(RedisKeys.AllProducts, result, TimeSpan.FromMinutes(5));
            return result;
        }

        public async Task<ProductResponseDto?> GetByIdAsync(int id)
        {
            var cacheKey = RedisKeys.Product(id);

            var cached = await _cache.GetAsync<ProductResponseDto>(cacheKey);
            if (cached is not null) return cached;

            var product = await _repository.GetByIdAsync(id);
            if (product == null) return null;

            var result = MapToResponse(product);

            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }

        public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto, int adminId)
        {
            string? imageUrl = null;

            if (dto.Image != null)
                imageUrl = await _cloudinaryService.UploadImageAsync(dto.Image, "shopnext/products");

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = imageUrl,
                CategoryId = dto.CategoryId,
                CreatedBy = adminId,
                IsCodAvailable = dto.IsCodAvailable
            };

            var created = await _repository.CreateAsync(product);

            await InvalidateProductCaches();

            return MapToResponse(created);
        }

        public async Task<ProductResponseDto?> UpdateAsync(int id, UpdateProductDto dto)
        {
            string? imageUrl = null;

            if (dto.Image != null)
                imageUrl = await _cloudinaryService.UploadImageAsync(dto.Image, "shopnext/products");

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = imageUrl,
                CategoryId = dto.CategoryId,
                IsActive = dto.IsActive,
                IsCodAvailable = dto.IsCodAvailable
            };

            var updated = await _repository.UpdateAsync(id, product);
            if (updated == null) return null;

            await InvalidateProductCaches(id);

            return MapToResponse(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _repository.DeleteAsync(id);

            if (result)
                await InvalidateProductCaches(id);

            return result;
        }

        private async Task InvalidateProductCaches(int? id = null)
        {
            await _cache.DeleteAsync(RedisKeys.AllProducts);

            if (id.HasValue)
                await _cache.DeleteAsync(RedisKeys.Product(id.Value));

            // Search cache 
        }

        private static ProductResponseDto MapToResponse(Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            ImageUrl = p.ImageUrl,
            AverageRating = p.AverageRating,
            ReviewCount = p.ReviewCount,
            IsActive = p.IsActive,
            DateCreated = p.DateCreated,
            CategoryName = p.Category != null ? p.Category.Name : string.Empty
        };
    }
}
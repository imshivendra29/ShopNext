using ShopNext.DTOs.Product;
using ShopNext.Models;
using ShopNext.Repositories.Interfaces;

namespace ShopNext.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductService(IProductRepository repository, ICloudinaryService cloudinaryService)
        {
            _repository = repository;
            _cloudinaryService = cloudinaryService;
        }
        public async Task<ProductSearchResponseDto> SearchAsync(ProductSearchDto dto)
        {
            if (dto.PageSize > 50) dto.PageSize = 50;

            var (products, totalCount) = await _repository.SearchAsync(
                dto.Keyword,
                dto.CategoryId,
                dto.MinPrice,
                dto.MaxPrice,
                dto.SortBy,
                dto.Page,
                dto.PageSize);

            var totalPages = (int)Math.Ceiling((double)totalCount / dto.PageSize);

            return new ProductSearchResponseDto
            {
                TotalItems = totalCount,
                TotalPages = totalPages,
                CurrentPage = dto.Page,
                PageSize = dto.PageSize,
                Products = products.Select(p => new ProductResponseDto
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
                    CategoryName = p.Category.Name
                }).ToList()
            };
        }
        public async Task<List<ProductResponseDto>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            return products.Select(p => new ProductResponseDto
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
                CategoryName = p.Category.Name
            }).ToList();
        }

        public async Task<ProductResponseDto?> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return null;

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                AverageRating = product.AverageRating,
                ReviewCount = product.ReviewCount,
                IsActive = product.IsActive,
                DateCreated = product.DateCreated,
                CategoryName = product.Category.Name
            };
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
                CreatedBy = adminId
            };

            var created = await _repository.CreateAsync(product);

            return new ProductResponseDto
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                Price = created.Price,
                Stock = created.Stock,
                ImageUrl = created.ImageUrl,
                AverageRating = created.AverageRating,
                ReviewCount = created.ReviewCount,
                IsActive = created.IsActive,
                DateCreated = created.DateCreated,
                CategoryName = created.Category.Name
            };
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
                IsActive = dto.IsActive
            };

            var updated = await _repository.UpdateAsync(id, product);
            if (updated == null) return null;

            return new ProductResponseDto
            {
                Id = updated.Id,
                Name = updated.Name,
                Description = updated.Description,
                Price = updated.Price,
                Stock = updated.Stock,
                ImageUrl = updated.ImageUrl,
                AverageRating = updated.AverageRating,
                ReviewCount = updated.ReviewCount,
                IsActive = updated.IsActive,
                DateCreated = updated.DateCreated,
                CategoryName = updated.Category.Name
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}

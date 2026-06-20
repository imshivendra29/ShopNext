using ShopNext.DTOs.Category;
using ShopNext.Infrastructure.Cloudinary.Interfaces;
using ShopNext.Models;
using ShopNext.Repositories.Interfaces;

namespace ShopNext.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly ICloudinaryService _cloudinaryService;

        public CategoryService(ICategoryRepository repository, ICloudinaryService cloudinaryService)
        {
            _repository = repository;
            _cloudinaryService = cloudinaryService;
        }


        public async Task<List<CategoryResponseDto>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();
            return categories.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                IsActive = c.IsActive,
                DateCreated = c.DateCreated
            }).ToList();
        }

        
        public async Task<CategoryResponseDto?> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return null;

            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                IsActive = category.IsActive,
                DateCreated = category.DateCreated
            };
        }

        //Description and ImageUrl are optional in CreateCategoryDto, so we need to handle null values when creating a new Category
        public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto)
        {
            string? imageUrl = null;
            if (dto.Image != null)
                imageUrl = await _cloudinaryService.UploadImageAsync(dto.Image, "shopnext/categories");

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = imageUrl
            };

            var created = await _repository.CreateAsync(category);

            return new CategoryResponseDto
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                ImageUrl = created.ImageUrl,
                IsActive = created.IsActive,
                DateCreated = created.DateCreated
            };
        }

        public async Task<CategoryResponseDto?> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            string? imageUrl = null;
            if (dto.Image != null)
                imageUrl = await _cloudinaryService.UploadImageAsync(dto.Image, "shopnext/categories");

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = imageUrl,
                IsActive = dto.IsActive
            };

            var updated = await _repository.UpdateAsync(id, category);
            if (updated == null) return null;

            return new CategoryResponseDto
            {
                Id = updated.Id,
                Name = updated.Name,
                Description = updated.Description,
                ImageUrl = updated.ImageUrl,
                IsActive = updated.IsActive,
                DateCreated = updated.DateCreated
            };
        }


        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}

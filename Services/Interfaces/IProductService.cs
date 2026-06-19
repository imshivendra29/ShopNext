using ShopNext.DTOs.Product;
using ShopNext.Models;

namespace ShopNext.Services
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllAsync();
        Task<ProductResponseDto?> GetByIdAsync(int id);
        Task<ProductResponseDto> CreateAsync(CreateProductDto dto, int adminId);
        Task<ProductResponseDto?> UpdateAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteAsync(int id);
        Task<ProductSearchResponseDto> SearchAsync(ProductSearchDto dto);
        
    }
}

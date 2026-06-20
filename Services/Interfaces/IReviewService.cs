using ShopNext.DTOs.Review;

namespace ShopNext.Services
{
    public interface IReviewService
    {
        Task<List<ReviewResponseDto>> GetByProductIdAsync(int productId);
        Task<ReviewResponseDto> CreateAsync(CreateReviewDto dto, int userId);
        Task<ReviewResponseDto?> UpdateAsync(int id, int userId, int rating, string? comment);
        Task<bool> DeleteAsync(int id, int userId);

    }
}

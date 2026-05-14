using ShopNext.DTOs.Cart;

namespace ShopNext.Services
{
    public interface ICartService
    {
        Task<CartResponseDto> GetCartAsync(int userId);
        Task AddItemAsync(int userId, AddToCartDto dto);
        Task UpdateItemAsync(int userId, UpdateCartItemDto dto);
        Task RemoveItemAsync(int userId, int productId);
        Task ClearCartAsync(int userId);
    }
}

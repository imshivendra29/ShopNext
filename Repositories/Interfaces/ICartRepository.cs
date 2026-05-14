using ShopNext.Models;
namespace ShopNext.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(int userId);
        Task<Cart> CreateCartAsync(int userId);
        Task AddItemToCartAsync(int userId, int productId, int quantity);
        Task UpdateItemQuantityAsync(int userId, int productId, int quantity);
        Task RemoveItemFromCartAsync(int userId, int productId);
        Task ClearCartAsync(int cartId);
    }
    
}

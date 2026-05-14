using ShopNext.DTOs.Cart;
using ShopNext.Repositories.Interfaces;
using ShopNext.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _repository;

    public CartService(ICartRepository repository)
    {
        _repository = repository;
    }

    public async Task<CartResponseDto> GetCartAsync(int userId)
    {
        var cart = await _repository.GetCartByUserIdAsync(userId);
        if (cart == null)
            return new CartResponseDto();

        var items = cart.CartItems.Select(ci => new CartItemResponseDto
        {
            ProductId = ci.ProductId,
            ProductName = ci.Product.Name,
            ProductImage = ci.Product.ImageUrl,
            Price = ci.Product.Price,
            Quantity = ci.Quantity,
            TotalPrice = ci.Product.Price * ci.Quantity
        }).ToList();

        return new CartResponseDto
        {
            CartId = cart.Id,
            Items = items,
            GrandTotal = items.Sum(i => i.TotalPrice)
        };
    }

    public async Task AddItemAsync(int userId, AddToCartDto dto)
    {
        await _repository.AddItemToCartAsync(userId, dto.ProductId, dto.Quantity);
    }

    public async Task UpdateItemAsync(int userId, UpdateCartItemDto dto)
    {
        await _repository.UpdateItemQuantityAsync(userId, dto.ProductId, dto.Quantity);
    }

    public async Task RemoveItemAsync(int userId, int productId)
    {
        await _repository.RemoveItemFromCartAsync(userId, productId);
    }

    public async Task ClearCartAsync(int userId)
    {
        var cart = await _repository.GetCartByUserIdAsync(userId);
        if (cart == null) return;
        await _repository.ClearCartAsync(cart.Id);
    }
}
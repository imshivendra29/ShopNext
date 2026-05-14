using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopNext.DTOs.Cart;
using ShopNext.Services;

namespace ShopNext.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]   // this is authentication, not authorization. It ensures the user is logged in, but doesn't check roles or permissions.
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            var cart = await _service.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddItem([FromBody] AddToCartDto dto)
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _service.AddItemAsync(userId, dto);
            return Ok(new { message = "Item added to cart" });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateItem([FromBody] UpdateCartItemDto dto)
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _service.UpdateItemAsync(userId, dto);
            return Ok(new { message = "Cart updated" });
        }

        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _service.RemoveItemAsync(userId, productId);
            return Ok(new { message = "Item removed" });
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _service.ClearCartAsync(userId);
            return Ok(new { message = "Cart cleared" });
        }
    }
}

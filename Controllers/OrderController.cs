using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopNext.DTOs.Order;
using ShopNext.Services;

namespace ShopNext.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            var order = await _service.PlaceOrderAsync(userId, dto);
            return Ok(order);
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            var orders = await _service.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            var order = await _service.GetOrderByIdAsync(id, userId);
            if (order == null) return NotFound(new { message = "Order not found" });
            return Ok(order);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var order = await _service.UpdateStatusAsync(id, status);
            if (order == null) return NotFound(new { message = "Order not found" });
            return Ok(order);
        }
    }
}

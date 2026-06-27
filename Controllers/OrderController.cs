using Microsoft.AspNetCore.Authorization;
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
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            IOrderService service,
            ILogger<OrderController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);

            _logger.LogInformation(
                "Checkout request received. UserId: {UserId}, PaymentMethod: {PaymentMethod}",
                userId,
                dto.PaymentMethod
            );

            var order = await _service.PlaceOrderAsync(userId, dto);

            _logger.LogInformation(
                "Checkout completed. UserId: {UserId}, OrderId: {OrderId}, Status: {Status}, PaymentStatus: {PaymentStatus}",
                userId,
                order.Id,
                order.Status,
                order.PaymentStatus
            );

            return Ok(order);
        }

        [HttpPost("verify-payment")]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentDto dto)
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);

            _logger.LogInformation(
                "Payment verification request received. UserId: {UserId}, OrderId: {OrderId}, RazorpayOrderId: {RazorpayOrderId}",
                userId,
                dto.OrderId,
                dto.RazorpayOrderId
            );

            var order = await _service.VerifyOnlinePaymentAsync(userId, dto);

            _logger.LogInformation(
                "Payment verified successfully. UserId: {UserId}, OrderId: {OrderId}, PaymentStatus: {PaymentStatus}",
                userId,
                order.Id,
                order.PaymentStatus
            );

            return Ok(new
            {
                message = "Payment verified successfully",
                order
            });
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);

            _logger.LogInformation("Fetching user orders. UserId: {UserId}", userId);

            var orders = await _service.GetUserOrdersAsync(userId);

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);

            _logger.LogInformation(
                "Fetching order by id. UserId: {UserId}, OrderId: {OrderId}",
                userId,
                id
            );

            var order = await _service.GetOrderByIdAsync(id, userId);

            if (order == null)
            {
                _logger.LogWarning(
                    "Order not found. UserId: {UserId}, OrderId: {OrderId}",
                    userId,
                    id
                );

                return NotFound(new { message = "Order not found" });
            }

            return Ok(order);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var adminId = int.Parse(User.FindFirst("uid")!.Value);

            _logger.LogInformation(
                "Admin updating order status. AdminId: {AdminId}, OrderId: {OrderId}, NewStatus: {Status}",
                adminId,
                id,
                status
            );

            var order = await _service.UpdateStatusAsync(id, status);

            if (order == null)
            {
                _logger.LogWarning(
                    "Order status update failed. Order not found. AdminId: {AdminId}, OrderId: {OrderId}",
                    adminId,
                    id
                );

                return NotFound(new { message = "Order not found" });
            }

            return Ok(order);
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopNext.DTOs.Order;
using ShopNext.Services;

namespace ShopNext.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IConfiguration _configuration;

        public PaymentController(
            IOrderService orderService,
            IConfiguration configuration)
        {
            _orderService = orderService;
            _configuration = configuration;
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentDto dto)
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);

            var order = await _orderService.VerifyOnlinePaymentAsync(userId, dto);

            return Ok(new
            {
                message = "Payment verified successfully",
                order
            });
        }

        [HttpGet("key")]
        public IActionResult GetRazorpayKey()
        {
            return Ok(new
            {
                keyId = _configuration["Razorpay:KeyId"]
            });
        }
    }
}
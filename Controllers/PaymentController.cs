using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopNext.DTOs.Payment;
using ShopNext.Infrastructure.Payment.Interfaces;
using ShopNext.Repositories.Interfaces;



namespace ShopNext.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IRazorpayService _razorpayService;
        private readonly IOrderRepository _orderRepository;
        private readonly IConfiguration _configuration;

        public PaymentController(
            IRazorpayService razorpayService,
            IOrderRepository orderRepository,
            IConfiguration configuration)
        {
            _razorpayService = razorpayService;
            _orderRepository = orderRepository;
            _configuration = configuration;
        }

        [HttpPost("initiate/{orderId}")]
        public async Task<IActionResult> InitiatePayment(int orderId)
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            var order = await _orderRepository.GetByIdAsync(orderId, userId);

            if (order == null)
                return NotFound(new { message = "Order not found" });

            if (order.PaymentStatus == "Paid")
                return BadRequest(new { message = "Order already paid" });

            if (order.PaymentMethod == "COD")
                return BadRequest(new { message = "COD order does not need payment" });

            var receipt = $"order_{orderId}_{userId}";
            var razorpayOrderId = await _razorpayService.CreateOrderAsync(
                order.TotalAmount, "INR", receipt);

           
            await _orderRepository.UpdateRazorpayOrderIdAsync(orderId, razorpayOrderId);

            return Ok(new InitiatePaymentResponseDto
            {
                RazorpayOrderId = razorpayOrderId,
                Amount = order.TotalAmount,
                Currency = "INR",
                KeyId = _configuration["Razorpay:KeyId"]!
            });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentDto dto)
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            var order = await _orderRepository.GetByIdAsync(dto.OrderId, userId);

            if (order == null)
                return NotFound(new { message = "Order not found" });

            var isValid = _razorpayService.VerifyPayment(
                dto.RazorpayOrderId,
                dto.RazorpayPaymentId,
                dto.RazorpaySignature);

            if (!isValid)
                return BadRequest(new { message = "Invalid payment signature" });

            await _orderRepository.UpdatePaymentStatusAsync(dto.OrderId, "Paid");
            await _orderRepository.UpdateStatusAsync(dto.OrderId, "Confirmed");

           
            var payment = await _orderRepository.GetPaymentByOrderIdAsync(dto.OrderId);
            if (payment != null)
            {
                payment.Status = "Success";
                payment.TransactionId = dto.RazorpayPaymentId;
                payment.PaidAt = DateTime.UtcNow;
                await _orderRepository.UpdatePaymentAsync(payment);
            }

            return Ok(new { message = "Payment verified successfully" });
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShopNext.Constants;
using ShopNext.DTOs.Otp;
using ShopNext.Repositories.Interfaces;
using ShopNext.Services.Interfaces;

namespace ShopNext.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OtpController : ControllerBase
    {
        private readonly IOtpService _service;
        private readonly IUserRepository _userRepository;
        public OtpController(IOtpService service, IUserRepository userRepository)
        {
            _service = service;
            _userRepository = userRepository;
        }
        [EnableRateLimiting(RateLimitPolicies.Otp)]
        [HttpPost("send")]
        public async Task<IActionResult> SendOtp()
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            if (string.IsNullOrEmpty(user.Phone))
                return BadRequest(new { message = "Please add phone number first" });

            await _service.SendOtpAsync(user.Phone);
            return Ok(new { message = "OTP sent successfully" });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var result = await _service.VerifyOtpAsync(dto.Phone, dto.Otp);
            return Ok(new { message = "Phone verified successfully" });
        }
    }
}

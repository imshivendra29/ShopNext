using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShopNext.Constants;
using ShopNext.DTOs.Auth;
using ShopNext.Services;

namespace ShopNext.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
           
                var token = await _authService.RegisterAsync(dto);
                return Ok(new { Token = token });
           
        }
        [EnableRateLimiting(RateLimitPolicies.Login)]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            
                var token = await _authService.LoginAsync(dto);
                return Ok(new { Token = token });
            
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopNext.DTOs.User;
using ShopNext.Exceptions;
using ShopNext.Services;

namespace ShopNext.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        private int GetUserId()
        {
            var uid = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(uid))
                throw new AppException("Unauthorized", 401);

            return int.Parse(uid);
        }
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await _userService.GetProfileAsync(GetUserId());
            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            await _userService.UpdateProfileAsync(GetUserId(), dto);
            return Ok(new { Message = "Profile updated" });
        }

        [HttpPatch("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            await _userService.ChangePasswordAsync(GetUserId(), dto);
            return Ok(new { Message = "Password changed" });
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount()
        {
            await _userService.DeleteAccountAsync(GetUserId());
            return Ok(new { Message = "Account deleted" });
        }
    }
}

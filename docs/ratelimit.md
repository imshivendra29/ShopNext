
Login Rate Limit
Policy: LoginPolicy
Limit: 3 requests / 10 minutes / IP
Response: 429 Too Many Requests
[EnableRateLimiting(RateLimitPolicies.Login)]
[HttpPost("login")]
OTP Rate Limit
Policy: OtpPolicy
Limit: 3 requests / 10 minutes / IP
OTP is sent to the phone number stored in the user's profile.
Phone number is NOT taken from request body or query params.
[EnableRateLimiting(RateLimitPolicies.Otp)]
[HttpPost("send")]

Required Header:

Authorization: Bearer <token>
Search Rate Limit
Policy: SearchPolicy
Limit: 60 requests / minute / IP
[EnableRateLimiting(RateLimitPolicies.Search)]
[HttpGet("search")]
Current Implementation
ASP.NET Core Built-in Rate Limiter
Fixed Window Algorithm
IP Based Partitioning
In-Memory Storage (Single Server)

Future Upgrade:

Single Server  -> In-Memory Rate Limiter
Multiple Server -> Redis Distributed Rate Limiter




example --> [EnableRateLimiting(RateLimitPolicies.Otp)]
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

using ShopNext.DTOs.Auth;

namespace ShopNext.Services
{
    public interface IAuthService
    {
        Task <string>RegisterAsync(RegisterDto dto);
        Task <string>LoginAsync(LoginDto dto);

    }
}

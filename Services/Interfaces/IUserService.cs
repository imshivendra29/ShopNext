using ShopNext.DTOs.User;

namespace ShopNext.Services
{
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync(int userId);
        Task UpdateProfileAsync(int userId, UpdateProfileDto dto);
        Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
        Task DeleteAccountAsync(int userId);
    }
}

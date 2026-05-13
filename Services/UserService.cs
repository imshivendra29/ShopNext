using ShopNext.DTOs.User;
using ShopNext.Exceptions;
using ShopNext.Repositories.Interfaces;

namespace ShopNext.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }
        public async Task<UserProfileDto> GetProfileAsync(int userId)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null)
                throw new AppException("User not found", 404);

            return new UserProfileDto
            {
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }
        public async Task UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null)
                throw new AppException("User not found", 404);

            user.Name = dto.Name;
            user.Email = dto.Email;
            await _repo.UpdateUserAsync(user);
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null)
                throw new AppException("User not found", 404);

            bool isValid = BCrypt.Net.BCrypt.Verify(
                dto.CurrentPassword,
                user.PasswordHash
            );
            if (!isValid)
                throw new AppException("Current password is incorrect", 400);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _repo.UpdateUserAsync(user);
        }

        public async Task DeleteAccountAsync(int userId)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null)
                throw new AppException("User not found", 404);

            await _repo.DeleteUserAsync(user);
        }
    }
}
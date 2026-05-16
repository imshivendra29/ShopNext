using ShopNext.DTOs.User;
using ShopNext.Exceptions;
using ShopNext.Repositories.Interfaces;

namespace ShopNext.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly ICloudinaryService _cloudinaryService;

        public UserService(IUserRepository repo, ICloudinaryService cloudinaryService)
        {
            _repo = repo;
            _cloudinaryService = cloudinaryService;
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
            if (dto == null)
                throw new AppException("Invalid profile data", 400);

            var user = await _repo.GetByIdAsync(userId);
            if (user == null)
                throw new AppException("User not found", 404);

            if (!string.IsNullOrEmpty(dto.Name))
                user.Name = dto.Name;

            if (dto.DateOfBirth.HasValue)
                user.DateOfBirth = dto.DateOfBirth;

            if (!string.IsNullOrEmpty(dto.Phone) && dto.Phone != user.Phone)
            {
                user.Phone = dto.Phone;
                user.IsPhoneVerified = false;
            }

            if (dto.ProfileImage != null)
                user.ProfileImageUrl = await _cloudinaryService
                    .UploadImageAsync(dto.ProfileImage, "shopnext/profiles");

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
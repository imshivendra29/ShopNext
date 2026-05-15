using ShopNext.DTOs.Auth;
using ShopNext.Exceptions;
using ShopNext.Helpers;
using ShopNext.Models;
using ShopNext.Repositories.Interfaces;

namespace ShopNext.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;
        private readonly JwtHelper _jwtHelper;

        public AuthService(
            IUserRepository repo,
            JwtHelper jwtHelper)
        {
            _repo = repo;
            _jwtHelper = jwtHelper;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _repo.GetByEmailAsync(dto.Email);

            if (existingUser != null)
                throw new AppException("User already exists", 409);

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            await _repo.AddUserAsync(user);
            return _jwtHelper.GenerateToken(user);
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _repo.GetByEmailAsync(dto.Email);

            if (user == null)
                throw new AppException("Invalid email or password", 401);

            bool isValid = BCrypt.Net.BCrypt.Verify(
                dto.Password,
                user.PasswordHash
            );

            if (!isValid)
                throw new AppException("Invalid email or password", 401);

            return _jwtHelper.GenerateToken(user);
        }

    }
}
using ShopNext.Exceptions;
using ShopNext.Models;
using ShopNext.Repositories.Interfaces;
using ShopNext.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ShopNext.Services.Implementations
{
    public class OtpService : IOtpService
    {
        private readonly IOtpRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public OtpService(
            IOtpRepository repository,
            IUserRepository userRepository,
            IConfiguration configuration,
            HttpClient httpClient)
        {
            _repository = repository;
            _userRepository = userRepository;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task SendOtpAsync(string phone)
        {
            var existing = await _repository.GetByPhoneAsync(phone);

            
            if (existing != null)
            {
                
                if ((DateTime.UtcNow - existing.LastRequestAt).TotalSeconds < 60)
                    throw new AppException("Please wait 1 minute before requesting another OTP", 429);

             
                if (existing.RequestCount >= 5 &&
                    (DateTime.UtcNow - existing.LastRequestAt).TotalHours < 1)
                    throw new AppException("Too many OTP requests. Try again after 1 hour", 429);

                
                if (existing.BlockedUntil.HasValue && existing.BlockedUntil > DateTime.UtcNow)
                    throw new AppException("Too many wrong attempts. Try again later", 429);
            }

          
            var otp = new Random().Next(100000, 999999).ToString();
            var otpHash = BCrypt.Net.BCrypt.HashPassword(otp);

            if (existing == null)
            {
                await _repository.CreateAsync(new OtpVerification
                {
                    Phone = phone,
                    OtpHash = otpHash,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                    LastRequestAt = DateTime.UtcNow,
                    RequestCount = 1
                });
            }
            else
            {
                existing.OtpHash = otpHash;
                existing.ExpiresAt = DateTime.UtcNow.AddMinutes(5);
                existing.IsUsed = false;
                existing.Attempts = 0;
                existing.LastRequestAt = DateTime.UtcNow;
                existing.RequestCount += 1;
                await _repository.UpdateAsync(existing);
            }

            
            await SendSmsAsync(phone, otp);
        }

        public async Task<bool> VerifyOtpAsync(string phone, string otp)
        {
            var existing = await _repository.GetByPhoneAsync(phone);

            if (existing == null)
                throw new AppException("OTP not found", 400);

            
            if (existing.BlockedUntil.HasValue && existing.BlockedUntil > DateTime.UtcNow)
                throw new AppException("Too many wrong attempts. Try again later", 429);

            
            if (existing.ExpiresAt < DateTime.UtcNow)
                throw new AppException("OTP expired", 400);

            
            if (existing.IsUsed)
                throw new AppException("OTP already used", 400);

            
            if (!BCrypt.Net.BCrypt.Verify(otp, existing.OtpHash))
            {
                existing.Attempts += 1;


                if (existing.Attempts >= 3)
                    existing.BlockedUntil = DateTime.UtcNow.AddMinutes(15);

                await _repository.UpdateAsync(existing);
                throw new AppException("Invalid OTP", 400);
            }

            
            existing.IsUsed = true;
            await _repository.UpdateAsync(existing);

            
            var user = await _userRepository.GetByPhoneAsync(phone);
            if (user != null)
            {
                user.IsPhoneVerified = true;
                await _userRepository.UpdateUserAsync(user);
            }

            return true;
        }

        private async Task SendSmsAsync(string phone, string otp)
        {
            var apiKey = _configuration["Fast2SMS:ApiKey"];
            var url = "https://www.fast2sms.com/dev/bulkV2";

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("authorization", apiKey);

            var body = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("variables_values", otp),
        new KeyValuePair<string, string>("route", "otp"),
        new KeyValuePair<string, string>("numbers", phone)
    });

            request.Content = body;
            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new AppException($"Failed to send OTP. Please try again.", 500);
        }
    }
}

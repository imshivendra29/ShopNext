using ShopNext.Models;
namespace ShopNext.Repositories.Interfaces
{
    public interface IOtpRepository
    {
        Task<OtpVerification?> GetByPhoneAsync(string phone);
        Task<OtpVerification> CreateAsync(OtpVerification otp);
        Task UpdateAsync(OtpVerification otp);

    }
}

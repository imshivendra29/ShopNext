namespace ShopNext.Services.Interfaces
{
    public interface IOtpService
    {
        Task SendOtpAsync(string phone);
        Task<bool> VerifyOtpAsync(string phone, string otp);
    }
}

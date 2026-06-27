using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using ShopNext.Exceptions;
using ShopNext.Infrastructure.Payment.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace ShopNext.Infrastructure.Payment.Implementations
{
    public class RazorpayService : IRazorpayService
    {
        private readonly string _keyId;
        private readonly string _keySecret;

        public RazorpayService(IConfiguration configuration)
        {
            _keyId = configuration["Razorpay:KeyId"]
                ?? throw new AppException("Razorpay KeyId missing", 500);

            _keySecret = configuration["Razorpay:KeySecret"]
                ?? throw new AppException("Razorpay KeySecret missing", 500);
        }

        public Task<string> CreateOrderAsync(decimal amount, string currency, string receipt)
        {
            try
            {
                var client = new RazorpayClient(_keyId, _keySecret);

                var amountInPaise = (int)Math.Round(amount * 100);

                var options = new Dictionary<string, object>
                {
                    { "amount", amountInPaise },
                    { "currency", currency },
                    { "receipt", receipt },
                    { "payment_capture", 1 }
                };

                var order = client.Order.Create(options);
                return Task.FromResult(order["id"].ToString());
            }
            catch (Exception ex)
            {
                throw new AppException($"Razorpay error: {ex.Message}", 500);
            }
        }

        public bool VerifyPayment(string orderId, string paymentId, string signature)
        {
            var payload = $"{orderId}|{paymentId}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_keySecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));

            var generatedSignature = Convert.ToHexString(hash).ToLowerInvariant();

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(generatedSignature),
                Encoding.UTF8.GetBytes(signature.ToLowerInvariant())
            );
        }
    }
}
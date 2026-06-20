namespace ShopNext.Infrastructure.Payment.Interfaces
{
    public interface IRazorpayService
    {
        Task<string> CreateOrderAsync(decimal amount, string currency, string receipt);
        bool VerifyPayment(string orderId, string paymentId, string signature);
    }
}

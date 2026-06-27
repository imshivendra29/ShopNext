
using ShopNext.Constants;
namespace ShopNext.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public string Method { get; set; } = PaymentMethods.COD;
        public string Status { get; set; } = PaymentStatuses.Pending;

        // Razorpay payment_id / COD reference
        public string? TransactionId { get; set; }

        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Order Order { get; set; } = null!;
    }
}
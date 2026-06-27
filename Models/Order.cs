using ShopNext.Models;
using ShopNext.Constants;
public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public string? RazorpayOrderId { get; set; }
    public string? RazorpayPaymentId { get; set; }

    public required string ShippingAddress { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = OrderStatuses.Pending;
    public string PaymentMethod { get; set; } = PaymentMethods.COD;
    public string PaymentStatus { get; set; } = PaymentStatuses.Unpaid;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public Payment? Payment { get; set; }
}
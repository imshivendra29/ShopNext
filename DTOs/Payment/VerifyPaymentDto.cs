using System.ComponentModel.DataAnnotations;

namespace ShopNext.DTOs.Order
{
    public class VerifyPaymentDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public required string RazorpayOrderId { get; set; }

        [Required]
        public required string RazorpayPaymentId { get; set; }

        [Required]
        public required string RazorpaySignature { get; set; }
    }
}
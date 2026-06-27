using System.ComponentModel.DataAnnotations;

namespace ShopNext.DTOs.Order
{
    public class PlaceOrderDto
    {
        [Required]
        [StringLength(500, MinimumLength = 10)]
        public required string ShippingAddress { get; set; }

        [Required]
        public required string PaymentMethod { get; set; }
    }
}
namespace ShopNext.DTOs.Order
{
    public class PlaceOrderDto
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "COD"; // COD ya Online
    }
}

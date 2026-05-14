namespace ShopNext.DTOs.Address
{
    public class UpdateAddressDto
    {
        public string Label { get; set; } = "Home";
        public string FullAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
    }
}

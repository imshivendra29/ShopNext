namespace ShopNext.DTOs.Address
{
    public class AddressResponseDto
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}

namespace ShopNext.Models
{
    public class Address
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Label { get; set; } = "Home";
        public string FullAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; } = null!;
    }
}
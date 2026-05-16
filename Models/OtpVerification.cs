namespace ShopNext.Models
{
    public class OtpVerification
    {
        public int Id { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string OtpHash { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public int Attempts { get; set; } = 0;
        public DateTime? BlockedUntil { get; set; }
        public DateTime LastRequestAt { get; set; } = DateTime.UtcNow;
        public int RequestCount { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

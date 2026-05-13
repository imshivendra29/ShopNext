namespace ShopNext.DTOs.Review
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime DateCreated { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}

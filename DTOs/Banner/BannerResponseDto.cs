namespace ShopNext.DTOs.Banners
{
    public class BannerResponseDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
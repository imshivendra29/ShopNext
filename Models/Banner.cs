namespace ShopNext.Models
{
    public class Banner
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}

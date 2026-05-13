namespace ShopNext.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } 
        public bool IsActive { get; set; } = true;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

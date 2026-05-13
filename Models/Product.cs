namespace ShopNext.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; } 
        public double AverageRating { get; set; } = 0;
        public int ReviewCount { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public int CategoryId { get; set; }
        public int CreatedBy { get; set; }

        public  Category Category { get; set; }=null!;
        public User CreatedByUser { get; set; } = null!;
        public ICollection<Review> Reviews { get; set; } = new List<Review>();



    }
}

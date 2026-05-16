namespace ShopNext.DTOs.Product
{
    public class ProductSearchResponseDto
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<ProductResponseDto> Products { get; set; } = new();
    }
}

namespace ShopNext.DTOs.Common
{
    public class PaginationDto
    {
        public int Page { get; set; } = 1;
        private int _pageSize = 20;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 100 ? 100 : value;
        }
    }
}

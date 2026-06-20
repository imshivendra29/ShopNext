namespace ShopNext.Infrastructure.Redis
{
    public static class RedisKeys
    {
        //banner
        public const string Banners = "banners:active";
        //product
        public const string AllProducts = "products:all";

        public static string Product(int id)
            => $"products:{id}";
        // search key me jitne bhi parameters hai unko include karna chahiye taki alag alag search ke liye alag alag cache ho
        public static string ProductSearch(
            string? keyword,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            int page,
            int pageSize)
        {
            return $"products:search:{keyword}:{categoryId}:{minPrice}:{maxPrice}:{sortBy}:{page}:{pageSize}";
        }
    }
}

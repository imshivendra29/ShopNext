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

        //phone otp 
        public static class Otp
        {
            public static string Hash(string phone) => $"otp:{phone}:hash";
            public static string Attempts(string phone) => $"otp:{phone}:attempts";
            public static string Cooldown(string phone) => $"otp:{phone}:cooldown";
            public static string Hourly(string phone) => $"otp:{phone}:hourly";
            public static string Blocked(string phone) => $"otp:{phone}:blocked";
        }
    }
}

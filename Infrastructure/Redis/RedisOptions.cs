namespace ShopNext.Infrastructure.Redis
{
    public class RedisOptions
    {
        public const string SectionName = "Redis";
        public string ConnectionString { get; set; } = string.Empty;
        public int DefaultTtlMinutes { get; set; } = 5;
    }
}

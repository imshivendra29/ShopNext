
namespace ShopNext.Constants
{
    public static class PaymentMethods
    {
        public const string COD = "COD";
        public const string Online = "Online";
    }

    public static class PaymentStatuses
    {
        public const string Pending = "Pending";
        public const string Paid = "Paid";
        public const string Failed = "Failed";
        public const string Unpaid = "Unpaid";
    }

    public static class OrderStatuses
    {
        public const string Pending = "Pending";
        public const string PaymentPending = "PaymentPending";
        public const string Confirmed = "Confirmed";
        public const string PaymentFailed = "PaymentFailed";
        public const string Cancelled = "Cancelled";

        public static string? Delivered { get; internal set; }
    }
}
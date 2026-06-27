using ShopNext.DTOs.Order;
using ShopNext.DTOs.Payment;

namespace ShopNext.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto> PlaceOrderAsync(int userId, PlaceOrderDto dto);
        Task<OrderResponseDto> VerifyOnlinePaymentAsync(int userId, VerifyPaymentDto dto);
        Task<OrderResponseDto?> GetOrderByIdAsync(int id, int userId);
        Task<List<OrderResponseDto>> GetUserOrdersAsync(int userId);
        Task<OrderResponseDto?> UpdateStatusAsync(int id, string status);
        Task<bool> HasUserPurchasedProductAsync(int userId, int productId);
    }
}
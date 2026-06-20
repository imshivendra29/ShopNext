using ShopNext.DTOs.Order;

namespace ShopNext.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto> PlaceOrderAsync(int userId, PlaceOrderDto dto);
        Task<OrderResponseDto?> GetOrderByIdAsync(int id, int userId);
        Task<List<OrderResponseDto>> GetUserOrdersAsync(int userId);
        Task<OrderResponseDto?> UpdateStatusAsync(int id, string status);

    }
}

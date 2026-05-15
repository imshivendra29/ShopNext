using ShopNext.DTOs.Order;
using ShopNext.Exceptions;
using ShopNext.Models;
using ShopNext.Repositories.Interfaces;

namespace ShopNext.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;

        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
        }

        public async Task<OrderResponseDto> PlaceOrderAsync(int userId, PlaceOrderDto dto)
        {
            // Cart fetch user need to fatch in server side because user can manipulate client side data
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
                throw new AppException("Cart is empty", 400);

           
            var orderItems = cart.CartItems.Select(ci => new OrderItem
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                Price = ci.Product.Price
            }).ToList();

            
            var total = orderItems.Sum(oi => oi.Price * oi.Quantity);

            // make - order
            var order = new Order
            {
                UserId = userId,
                ShippingAddress = dto.ShippingAddress,
                TotalAmount = total,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = "Unpaid",
                Status = "Pending",
                OrderItems = orderItems
            };

            // send payment info---
            order.Payment = new Payment
            {
                Amount = total,
                Method = dto.PaymentMethod,
                Status = "Pending"
            };

            var created = await _orderRepository.CreateAsync(order);

            // auto cart clear after order place
            await _cartRepository.ClearCartAsync(cart.Id);

            return MapToDto(created);
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int id, int userId)
        {
            var order = await _orderRepository.GetByIdAsync(id, userId);
            if (order == null) return null;
            return MapToDto(order);
        }

        public async Task<List<OrderResponseDto>> GetUserOrdersAsync(int userId)
        {
            var orders = await _orderRepository.GetUserOrdersAsync(userId);
            return orders.Select(MapToDto).ToList();
        }

        public async Task<OrderResponseDto?> UpdateStatusAsync(int id, string status)
        {
            var order = await _orderRepository.UpdateStatusAsync(id, status);
            if (order == null) return null;
            return MapToDto(order);
        }

        
        private OrderResponseDto MapToDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                ShippingAddress = order.ShippingAddress,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    TotalPrice = oi.Price * oi.Quantity
                }).ToList()
            };
        }
    }
}

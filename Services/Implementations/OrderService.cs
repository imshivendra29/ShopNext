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
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IUserRepository userRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }

        public async Task<OrderResponseDto> PlaceOrderAsync(int userId, PlaceOrderDto dto)
        {
            
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new AppException("User not found", 404);

            if (user.Phone == null)
                throw new AppException("Please add phone number before ordering", 400);

            if (!user.IsPhoneVerified)
                throw new AppException("Please verify your phone number before ordering", 400);

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
                throw new AppException("Cart is empty", 400);

            
            var productIds = cart.CartItems.Select(ci => ci.ProductId).ToList();
            var freshPrices = await _productRepository.GetFreshPricesAsync(productIds);

            var orderItems = cart.CartItems.Select(ci =>
            {
                
                if (!freshPrices.TryGetValue(ci.ProductId, out var price))
                    throw new AppException(
                        $"'{ci.Product.Name}' is no longer available", 400);

                return new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = price  // fresh DB price
                };
            }).ToList();

            // stock deduct -
            foreach (var item in cart.CartItems)
            {
                var deducted = await _productRepository.DeductStockAsync(
                    item.ProductId, item.Quantity);

                if (!deducted)
                    throw new AppException(
                        $"'{item.Product.Name}' out of stock", 400);
            }

            var total = orderItems.Sum(oi => oi.Price * oi.Quantity);

            var order = new Order
            {
                UserId = userId,
                ShippingAddress = dto.ShippingAddress,
                TotalAmount = total,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = "Unpaid",
                Status = "Pending",
                OrderItems = orderItems,
                Payment = new Payment
                {
                    Amount = total,
                    Method = dto.PaymentMethod,
                    Status = "Pending"
                }
            };

            var created = await _orderRepository.CreateAsync(order);
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
        public async Task<bool> HasUserPurchasedProductAsync(int userId, int productId)
        {
            return await _orderRepository.HasUserPurchasedProductAsync(userId, productId);
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
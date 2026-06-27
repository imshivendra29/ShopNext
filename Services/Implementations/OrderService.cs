using ShopNext.Constants;
using ShopNext.DTOs.Order;
using ShopNext.Exceptions;
using ShopNext.Infrastructure.Payment.Interfaces;
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
        private readonly IRazorpayService _razorpayService;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IUserRepository userRepository,
            IProductRepository productRepository,
            IRazorpayService razorpayService,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _razorpayService = razorpayService;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderResponseDto> PlaceOrderAsync(int userId, PlaceOrderDto dto)
        {
            await ValidateUserAsync(userId);

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
                throw new AppException("Cart is empty", 400);
            
            if (dto.PaymentMethod != PaymentMethods.COD &&
                dto.PaymentMethod != PaymentMethods.Online)
                throw new AppException("Invalid payment method", 400);

            var productIds = cart.CartItems.Select(ci => ci.ProductId).ToList();
            var products = await _productRepository.GetProductsByIdsAsync(productIds);

            if (products.Count != productIds.Count)
                throw new AppException("Some products are no longer available", 400);

            if (products.Any(p => !p.IsActive))
                throw new AppException("Some products are currently inactive", 400);

            if (dto.PaymentMethod == PaymentMethods.COD &&
                products.Any(p => !p.IsCodAvailable))
                throw new AppException("Some products are only available for online payment", 400);

            var orderItems = cart.CartItems.Select(ci =>
            {
                var product = products.First(p => p.Id == ci.ProductId);

                return new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = product.Price
                };
            }).ToList();

            foreach (var item in orderItems)
            {
                var product = products.First(p => p.Id == item.ProductId);

                if (product.Stock < item.Quantity)
                    throw new AppException($"'{product.Name}' is out of stock", 400);
            }

            var total = orderItems.Sum(x => x.Price * x.Quantity);

            return dto.PaymentMethod == PaymentMethods.COD
                ? await PlaceCodOrderAsync(userId, cart.Id, dto.ShippingAddress, orderItems, total)
                : await CreateOnlineOrderAsync(userId, dto.ShippingAddress, orderItems, total);
        }

        private async Task<OrderResponseDto> PlaceCodOrderAsync(
            int userId,
            int cartId,
            string shippingAddress,
            List<OrderItem> orderItems,
            decimal total)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                foreach (var item in orderItems)
                {
                    var deducted = await _productRepository.DeductStockAsync(item.ProductId, item.Quantity);

                    if (!deducted)
                        throw new AppException("Product out of stock", 400);
                }
                //var isRollbackTest = true;
                //if (isRollbackTest)
                //    throw new Exception("Transaction rollback test");
                var order = new Order
                {
                    UserId = userId,
                    ShippingAddress = shippingAddress,
                    TotalAmount = total,
                    PaymentMethod = PaymentMethods.COD,
                    PaymentStatus = PaymentStatuses.Unpaid,
                    Status = OrderStatuses.Confirmed,
                    OrderItems = orderItems,
                    Payment = new Payment
                    {
                        Amount = total,
                        Method = PaymentMethods.COD,
                        Status = PaymentStatuses.Unpaid
                    }
                };

                var created = await _orderRepository.CreateAsync(order);
                await _cartRepository.ClearCartAsync(cartId);

                await _unitOfWork.CommitAsync();

                return MapToDto(created);
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        private async Task<OrderResponseDto> CreateOnlineOrderAsync(
            int userId,
            string shippingAddress,
            List<OrderItem> orderItems,
            decimal total)
        {
            var order = new Order
            {
                UserId = userId,
                ShippingAddress = shippingAddress,
                TotalAmount = total,
                PaymentMethod = PaymentMethods.Online,
                PaymentStatus = PaymentStatuses.Pending,
                Status = OrderStatuses.PaymentPending,
                OrderItems = orderItems,
                Payment = new Payment
                {
                    Amount = total,
                    Method = PaymentMethods.Online,
                    Status = PaymentStatuses.Pending
                }
            };

            var created = await _orderRepository.CreateAsync(order);

            var razorpayOrderId = await _razorpayService.CreateOrderAsync(
                total,
                "INR",
                $"order_{created.Id}"
            );

            created.RazorpayOrderId = razorpayOrderId;

            await _unitOfWork.SaveChangesAsync();

            return MapToDto(created);
        }

        public async Task<OrderResponseDto> VerifyOnlinePaymentAsync(int userId, VerifyPaymentDto dto)
        {
            var transactionCompleted = false;
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var order = await _orderRepository.GetByIdAsync(dto.OrderId, userId);

                if (order == null)
                    throw new AppException("Order not found", 404);

                if (order.PaymentMethod != PaymentMethods.Online)
                    throw new AppException("This is not an online payment order", 400);

                if (order.Status != OrderStatuses.PaymentPending)
                    throw new AppException("Order is not pending for payment", 400);

                if (order.RazorpayOrderId != dto.RazorpayOrderId)
                    throw new AppException("Invalid Razorpay order id", 400);

                var isValid = _razorpayService.VerifyPayment(
                    dto.RazorpayOrderId,
                    dto.RazorpayPaymentId,
                    dto.RazorpaySignature
                );

                if (!isValid)
                {
                    order.Status = OrderStatuses.PaymentFailed;
                    order.PaymentStatus = PaymentStatuses.Failed;

                    if (order.Payment != null)
                        order.Payment.Status = PaymentStatuses.Failed;

                    await _unitOfWork.CommitAsync();
                    transactionCompleted = true;

                    throw new AppException("Payment verification failed", 400);
                }

                foreach (var item in order.OrderItems)
                {
                    var deducted = await _productRepository.DeductStockAsync(item.ProductId, item.Quantity);

                    if (!deducted)
                        throw new AppException($"'{item.Product?.Name}' out of stock", 400);
                }

                order.Status = OrderStatuses.Confirmed;
                order.PaymentStatus = PaymentStatuses.Paid;

                if (order.Payment != null)
                {
                    order.Payment.Status = PaymentStatuses.Paid;
                    order.Payment.TransactionId = dto.RazorpayPaymentId;
                    order.Payment.PaidAt = DateTime.UtcNow;
                }

                await _cartRepository.ClearCartByUserIdAsync(userId);

                await _unitOfWork.CommitAsync();
                transactionCompleted = true;

                return MapToDto(order);
            }
            catch
            {
                if (!transactionCompleted)
                    await _unitOfWork.RollbackAsync();

                throw;
            }
        }

        private async Task ValidateUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new AppException("User not found", 404);

            if (string.IsNullOrWhiteSpace(user.Phone))
                throw new AppException("Please add phone number before ordering", 400);

            if (!user.IsPhoneVerified)
                throw new AppException("Please verify your phone number before ordering", 400);
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int id, int userId)
        {
            var order = await _orderRepository.GetByIdAsync(id, userId);
            return order == null ? null : MapToDto(order);
        }

        public async Task<List<OrderResponseDto>> GetUserOrdersAsync(int userId)
        {
            var orders = await _orderRepository.GetUserOrdersAsync(userId);
            return orders.Select(MapToDto).ToList();
        }

        public async Task<OrderResponseDto?> UpdateStatusAsync(int id, string status)
        {
            var order = await _orderRepository.UpdateStatusAsync(id, status);
            return order == null ? null : MapToDto(order);
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
                RazorpayOrderId = order.RazorpayOrderId,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? string.Empty,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    TotalPrice = oi.Price * oi.Quantity
                }).ToList()
            };
        }
    }
}
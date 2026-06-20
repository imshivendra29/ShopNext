using Microsoft.AspNetCore.Http.HttpResults;
using ShopNext.DTOs.Review;
using ShopNext.Exceptions;
using ShopNext.Models;
using ShopNext.Repositories.Implementations;
using ShopNext.Repositories.Interfaces;
using ShopNext.Services.Implementations;

namespace ShopNext.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public ReviewService(IReviewRepository repository, IProductRepository productRepository, IOrderRepository orderRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public async Task<List<ReviewResponseDto>> GetByProductIdAsync(int productId)
        {
            var reviews = await _repository.GetByProductIdAsync(productId);
            return reviews.Select(r => new ReviewResponseDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                DateCreated = r.DateCreated,
                UserName = r.User.Name
            }).ToList();
        }

        public async Task<ReviewResponseDto> CreateAsync(CreateReviewDto dto, int userId)
        {
            var hasPurchased = await _orderRepository.HasUserPurchasedProductAsync(userId, dto.ProductId);
            if (!hasPurchased)
                throw new AppException("You can only review products you have purchased", 400);

            var existing = await _repository.GetByUserAndProductAsync(userId, dto.ProductId);
            if (existing != null)
                throw new AppException("You have already reviewed this product", 400);

            var review = new Review
            {
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                UserId = userId
            };

            var created = await _repository.CreateAsync(review);

            var product = await _productRepository.GetByIdAsync(dto.ProductId);
            if (product != null)
            {
                product.ReviewCount += 1;
                product.AverageRating = ((product.AverageRating * (product.ReviewCount - 1)) + dto.Rating) / product.ReviewCount;
                await _productRepository.UpdateAsync(product.Id, product);
            }

            return new ReviewResponseDto
            {
                Id = created.Id,
                Rating = created.Rating,
                Comment = created.Comment,
                DateCreated = created.DateCreated,
                UserName = created.User.Name
            };
        }
        public async Task<ReviewResponseDto?> UpdateAsync(int id, int userId, int rating, string? comment)
        {
            var existing = await _repository.GetByIdAsync(id, userId);
            if (existing == null) return null;

            var oldRating = existing.Rating;

            var updated = await _repository.UpdateAsync(id, userId, rating, comment);
            if (updated == null) return null;

            var product = await _productRepository.GetByIdAsync(existing.ProductId);
            if (product != null && product.ReviewCount > 0)
            {
                product.AverageRating = ((product.AverageRating * product.ReviewCount) - oldRating + rating) / product.ReviewCount;
                await _productRepository.UpdateAsync(product.Id, product);
            }

            return new ReviewResponseDto
            {
                Id = updated.Id,
                Rating = updated.Rating,
                Comment = updated.Comment,
                DateCreated = updated.DateCreated,
                UserName = updated.User.Name
            };
        }
        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var existing = await _repository.GetByIdAsync(id, userId);
            if (existing == null) return false;

            var oldRating = existing.Rating;
            var productId = existing.ProductId;

            var deleted = await _repository.DeleteAsync(id, userId);
            if (!deleted) return false;

            var product = await _productRepository.GetByIdAsync(productId);
            if (product != null && product.ReviewCount > 0)
            {
                product.AverageRating = product.ReviewCount == 1
                    ? 0
                    : ((product.AverageRating * product.ReviewCount) - oldRating) / (product.ReviewCount - 1);

                product.ReviewCount -= 1;
                await _productRepository.UpdateAsync(product.Id, product);
            }

            return true;
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopNext.DTOs.Review;
using ShopNext.Services;

namespace ShopNext.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _service;

        public ReviewController(IReviewService service)
        {
            _service = service;
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var reviews = await _service.GetByProductIdAsync(productId);
            return Ok(reviews);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            var review = await _service.CreateAsync(dto, userId);
            return Ok(review);
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] CreateReviewDto dto)
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            var result = await _service.UpdateAsync(id, userId, dto.Rating, dto.Comment);
            if (result == null) return NotFound(new { message = "Review not found or unauthorized" });
            return Ok(result);
        }
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            var result = await _service.DeleteAsync(id, userId);
            if (!result) return NotFound(new { message = "Review not found or unauthorized" });
            return Ok(new { message = "Review deleted" });
        }
    }
}

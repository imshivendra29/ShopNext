using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopNext.Services.Interfaces;
using ShopNext.DTOs.Banner;
namespace ShopNext.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly IBannerService _bannerService;

        public BannerController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _bannerService.GetAllAsync();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateBannerDto dto)
        {
            var banner = await _bannerService.CreateAsync(dto.Image);
            return Ok(banner);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _bannerService.DeleteAsync(id);
            return Ok(new { message = "Banner deleted" });
        }
    }
    }

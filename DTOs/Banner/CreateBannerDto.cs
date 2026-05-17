using Microsoft.AspNetCore.Http;

namespace ShopNext.DTOs.Banner
{
    public class CreateBannerDto
    {
        public IFormFile Image { get; set; }
    }
}
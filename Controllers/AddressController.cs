using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopNext.DTOs.Address;
using ShopNext.Services;

namespace ShopNext.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _service;

        public AddressController(IAddressService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyAddresses()
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            var addresses = await _service.GetMyAddressesAsync(userId);
            return Ok(addresses);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAddressDto dto)
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            var address = await _service.CreateAsync(userId, dto);
            return Ok(address);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAddressDto dto)
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            var address = await _service.UpdateAsync(id, userId, dto);
            if (address == null) return NotFound(new { message = "Address not found" });
            return Ok(address);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            var result = await _service.DeleteAsync(id, userId);
            if (!result) return NotFound(new { message = "Address not found" });
            return Ok(new { message = "Address deleted" });
        }

        [HttpPatch("{id}/default")]
        public async Task<IActionResult> SetDefault(int id)
        {
            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _service.SetDefaultAsync(id, userId);
            return Ok(new { message = "Default address updated" });
        }
    }
}

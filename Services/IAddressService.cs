using ShopNext.DTOs.Address;
namespace ShopNext.Services
{
    public interface IAddressService
    {
        Task<List<AddressResponseDto>> GetMyAddressesAsync(int userId);
        Task<AddressResponseDto> CreateAsync(int userId, CreateAddressDto dto);
        Task<AddressResponseDto?> UpdateAsync(int id, int userId, UpdateAddressDto dto);
        Task<bool> DeleteAsync(int id, int userId);
        Task SetDefaultAsync(int id, int userId);
    }
}

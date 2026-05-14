using ShopNext.DTOs.Address;
using ShopNext.Models;
using ShopNext.Repositories.Interfaces;

namespace ShopNext.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _repository;

        public AddressService(IAddressRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<AddressResponseDto>> GetMyAddressesAsync(int userId)
        {
            var addresses = await _repository.GetByUserIdAsync(userId);
            return addresses.Select(a => new AddressResponseDto
            {
                Id = a.Id,
                Label = a.Label,
                FullAddress = a.FullAddress,
                City = a.City,
                State = a.State,
                PinCode = a.PinCode,
                IsDefault = a.IsDefault
            }).ToList();
        }

        public async Task<AddressResponseDto> CreateAsync(int userId, CreateAddressDto dto)
        {
            var address = new Address
            {
                UserId = userId,
                Label = dto.Label,
                FullAddress = dto.FullAddress,
                City = dto.City,
                State = dto.State,
                PinCode = dto.PinCode,
                IsDefault = dto.IsDefault
            };

            // Agar pehli address hai toh automatically default set karo
            var existing = await _repository.GetByUserIdAsync(userId);
            if (!existing.Any())
                address.IsDefault = true;

            var created = await _repository.CreateAsync(address);

            return new AddressResponseDto
            {
                Id = created.Id,
                Label = created.Label,
                FullAddress = created.FullAddress,
                City = created.City,
                State = created.State,
                PinCode = created.PinCode,
                IsDefault = created.IsDefault
            };
        }

        public async Task<AddressResponseDto?> UpdateAsync(int id, int userId, UpdateAddressDto dto)
        {
            var address = new Address
            {
                Label = dto.Label,
                FullAddress = dto.FullAddress,
                City = dto.City,
                State = dto.State,
                PinCode = dto.PinCode,
                IsDefault = dto.IsDefault
            };

            var updated = await _repository.UpdateAsync(id, userId, address);
            if (updated == null) return null;

            return new AddressResponseDto
            {
                Id = updated.Id,
                Label = updated.Label,
                FullAddress = updated.FullAddress,
                City = updated.City,
                State = updated.State,
                PinCode = updated.PinCode,
                IsDefault = updated.IsDefault
            };
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            return await _repository.DeleteAsync(id, userId);
        }

        public async Task SetDefaultAsync(int id, int userId)
        {
            await _repository.SetDefaultAsync(id, userId);
        }
    }
}

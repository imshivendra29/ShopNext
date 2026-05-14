using ShopNext.Models;
namespace ShopNext.Repositories.Interfaces
{
    public interface IAddressRepository
    {
        Task<List<Address>> GetByUserIdAsync(int userId);
        Task<Address?> GetByIdAsync(int id, int userId);
        Task<Address> CreateAsync(Address address);
        Task<Address?> UpdateAsync(int id, int userId, Address address);
        Task<bool> DeleteAsync(int id, int userId);
        Task SetDefaultAsync(int id, int userId);
    }
}

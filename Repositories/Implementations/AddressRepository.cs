using ShopNext.Data;
using ShopNext.Models;
using ShopNext.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace ShopNext.Repositories.Implementations
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ShopNextDbContext _context;

        public AddressRepository(ShopNextDbContext context)
        {
            _context = context;
        }

        public async Task<List<Address>> GetByUserIdAsync(int userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ToListAsync();
        }

        public async Task<Address?> GetByIdAsync(int id, int userId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        }

        public async Task<Address> CreateAsync(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<Address?> UpdateAsync(int id, int userId, Address address)
        {
            var existing = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            if (existing == null) return null;

            existing.Label = address.Label;
            existing.FullAddress = address.FullAddress;
            existing.City = address.City;
            existing.State = address.State;
            existing.PinCode = address.PinCode;
            existing.IsDefault = address.IsDefault;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            if (address == null) return false;

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task SetDefaultAsync(int id, int userId)
        {
            // Pehle sab addresses ka IsDefault false karo
            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            foreach (var a in addresses)
                a.IsDefault = false;

            // Phir selected address ka IsDefault true karo
            var selected = addresses.FirstOrDefault(a => a.Id == id);
            if (selected != null)
                selected.IsDefault = true;

            await _context.SaveChangesAsync();
        }
    }
}

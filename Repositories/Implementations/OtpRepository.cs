using ShopNext.Data;
using ShopNext.Models;
using ShopNext.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;



namespace ShopNext.Repositories.Implementations
{
    public class OtpRepository : IOtpRepository
    {
        private readonly ShopNextDbContext _context;

        public OtpRepository(ShopNextDbContext context)
        {
            _context = context;
        }

        public async Task<OtpVerification?> GetByPhoneAsync(string phone)
        {
            return await _context.OtpVerifications
                .FirstOrDefaultAsync(o => o.Phone == phone);
        }

        public async Task<OtpVerification> CreateAsync(OtpVerification otp)
        {
            _context.OtpVerifications.Add(otp);
            await _context.SaveChangesAsync();
            return otp;
        }

        public async Task UpdateAsync(OtpVerification otp)
        {
            _context.OtpVerifications.Update(otp);
            await _context.SaveChangesAsync();
        }
    }
}

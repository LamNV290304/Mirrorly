using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Services
{
    public class ProfileServices : IProfileServices
    {
        private readonly ProjectExeContext _context;

        public ProfileServices(ProjectExeContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateCustomerProfile(CustomerProfile profile)
        {
            try
            {
                _context.CustomerProfiles.Add(profile);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateMuaProfile(Muaprofile profile)
        {
            try
            {
                _context.Muaprofiles.Add(profile);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateCustomerProfile(CustomerProfile profile)
        {
            try
            {
                _context.CustomerProfiles.Update(profile);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateMuaProfile(Muaprofile profile)
        {
            try
            {
                _context.Muaprofiles.Update(profile);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<CustomerProfile?> GetCustomerProfile(int userId)
        {
            return await _context.CustomerProfiles
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(c => c.CustomerId == userId);
        }

        public async Task<Muaprofile?> GetMuaProfile(int userId)
        {
            return await _context.Muaprofiles
                .Include(m => m.Mua)
                .Include(m => m.Services)
                .FirstOrDefaultAsync(m => m.Muaid == userId);
        }
    }
}
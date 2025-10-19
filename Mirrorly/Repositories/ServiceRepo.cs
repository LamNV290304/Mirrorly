using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Repositories.Interfaces;

namespace Mirrorly.Repositories
{
    public class ServiceRepo : IServiceRepo
    {
        private readonly ProjectExeContext _context;
        public ServiceRepo(ProjectExeContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            return await _context.Services
                                 .Include(s => s.Category)
                                 .FirstOrDefaultAsync(s => s.ServiceId == id);
        }

        public List<Service> getServicesByMuaId(int muaId)
        {
            return _context.Services.Include(c => c.Category).Where(s => s.MuaId == muaId).ToList();
        }

        public async Task UpdateAsync(Service service)
        {
            var existing = await _context.Services.FindAsync(service.ServiceId);

            if (existing != null)
            {
                existing.Name = service.Name;
                existing.Description = service.Description;
                existing.BasePrice = service.BasePrice;
                existing.Currency = service.Currency;
                existing.DurationMin = service.DurationMin;
                existing.CategoryId = service.CategoryId;
                existing.Active = service.Active;
                existing.ImageUrl = service.ImageUrl;
                await _context.SaveChangesAsync();
            }
        }

    }
}

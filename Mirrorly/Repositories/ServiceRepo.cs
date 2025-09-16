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
        public List<Service> getServicesByMuaId(int muaId)
        {
            return _context.Services.Include(c => c.Category).Where(s => s.MuaId == muaId).ToList();
        }
    }
}

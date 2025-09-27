using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Repositories.Interfaces;

namespace Mirrorly.Repositories
{
    public class PortfoRepo : IPortfoRepo
    {
        private readonly ProjectExeContext _context;

        public PortfoRepo(ProjectExeContext context)
        {
            _context = context;
        }

        public List<PortfolioItem> getPortfoliosById(int muaId)
        {
            return _context.PortfolioItems.Where(p => p.Muaid == muaId).ToList();
        }
    }
}

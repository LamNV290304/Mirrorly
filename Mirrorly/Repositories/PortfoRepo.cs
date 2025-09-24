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

        public async Task<PortfolioItem?> GetByIdAsync(int id)
        {
            return await _context.PortfolioItems.Include(m => m.Mua).FirstOrDefaultAsync(p => p.ItemId == id);
        }

        public List<PortfolioItem> getPortfoliosById(int muaId)
        {
            return _context.PortfolioItems.Where(p => p.Muaid == muaId).ToList();
        }

        public async Task UpdateAsync(PortfolioItem portfolio)
        {
            var existing = await _context.PortfolioItems.FindAsync(portfolio.ItemId);
            if (existing != null)
            {
                existing.Title = portfolio.Title;
                existing.Description = portfolio.Description;
                existing.MediaUrl = portfolio.MediaUrl;
                existing.CreatedAtUtc = portfolio.CreatedAtUtc;
                await _context.SaveChangesAsync();
            }
        }
    }
}

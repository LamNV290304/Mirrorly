using Mirrorly.Models;

namespace Mirrorly.Repositories.Interfaces
{
    public interface IPortfoRepo
    {
        public List<PortfolioItem> getPortfoliosById(int muaId);
        Task<PortfolioItem?> GetByIdAsync(int id);
        Task UpdateAsync(PortfolioItem portfolio);
    }
}

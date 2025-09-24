using Mirrorly.Models;

namespace Mirrorly.Services.Interfaces
{
    public interface IPortfoServices
    {
        public List<PortfolioItem> getPortfolioItemsByMuaId(int id);
        Task<PortfolioItem?> GetPortfolioByIdAsync(int id);
        Task UpdatePortfoAsync(PortfolioItem portfo);
    }
}

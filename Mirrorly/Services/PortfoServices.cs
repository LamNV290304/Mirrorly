using Mirrorly.Models;
using Mirrorly.Repositories.Interfaces;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Services
{
    public class PortfoServices : IPortfoServices
    {
        private readonly IPortfoRepo _repo;

        public PortfoServices(IPortfoRepo repo)
        {
            _repo = repo;
        }

        public async Task<PortfolioItem?> GetPortfolioByIdAsync(int id)
        {
           return await _repo.GetByIdAsync(id);
        }

        public List<PortfolioItem> getPortfolioItemsByMuaId(int id)
        {
            return _repo.getPortfoliosById(id);
        }

        public async Task UpdatePortfoAsync(PortfolioItem portfo)
        {
            await _repo.UpdateAsync(portfo);
        }
    }
}

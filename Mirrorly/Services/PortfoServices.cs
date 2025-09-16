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

        public List<PortfolioItem> getPortfolioItemsByMuaId(int id)
        {
            return _repo.getPortfoliosById(id);
        }
    }
}
